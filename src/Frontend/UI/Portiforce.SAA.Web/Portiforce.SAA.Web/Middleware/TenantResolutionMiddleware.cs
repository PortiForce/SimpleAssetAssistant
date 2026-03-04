using Microsoft.Extensions.Options;
using Portiforce.SAA.Contracts.Models.Client;
using Portiforce.SAA.Contracts.Services;
using Portiforce.SAA.Web.Security;
using Portiforce.SAA.Web.Services;

namespace Portiforce.SAA.Web.Middleware;

public sealed class TenantResolutionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly TenancyOptions _options;

	public TenantResolutionMiddleware(RequestDelegate next, IOptions<TenancyOptions> options)
	{
		_next = next;
		_options = options.Value;
	}

	public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, ITenantResolver tenantResolver)
	{
		var host = context.Request.Host.Host;

		// 1) Landing host (no tenant)
		if (string.Equals(host, _options.BaseDomain, StringComparison.OrdinalIgnoreCase) ||
			string.Equals(host, $"www.{_options.BaseDomain}", StringComparison.OrdinalIgnoreCase))
		{
			tenantContext.IsLanding = true;
			tenantContext.Prefix = null;
			tenantContext.TenantId = null;

			await _next(context);
			return;
		}

		// 2) Tenant host: {prefix}.{BaseDomain}
		var suffix = "." + _options.BaseDomain;
		if (host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
		{
			var prefix = host[..^suffix.Length];

			// Basic prefix validation (no nested subdomains)
			if (string.IsNullOrWhiteSpace(prefix) || prefix.Contains('.'))
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync("Invalid tenant prefix.");
				return;
			}

			tenantContext.IsLanding = false;
			tenantContext.Prefix = prefix;

			TenantResolution? resolved = await tenantResolver.ResolveByPrefixAsync(prefix, context.RequestAborted);
			if (resolved is null)
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.WriteAsync("Unknown tenant.");
				return;
			}

			tenantContext.TenantId = resolved.TenantId;
			tenantContext.PublicName = resolved.Name;

			await _next(context);
			return;
		}

		// 3) Invalid host for this environment
		context.Response.StatusCode = StatusCodes.Status404NotFound;
		await context.Response.WriteAsync("Unknown tenant host.");
	}
}
