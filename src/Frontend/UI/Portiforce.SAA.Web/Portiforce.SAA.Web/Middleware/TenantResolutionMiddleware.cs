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
		this._next = next;
		this._options = options.Value;
	}

	public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, ITenantResolver tenantResolver)
	{
		string host = context.Request.Host.Host;

		// 1) Landing host (no tenant)
		if (this.IsBaseDomain(host))
		{
			tenantContext.IsLanding = true;
			await this._next(context);
			return;
		}

		// 2) Tenant host: {prefix}.{BaseDomain}
		string suffix = "." + this._options.BaseDomain;
		if (host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
		{
			string prefix = host[..^suffix.Length];

			// Basic prefix validation (no nested subdomains)
			if (string.IsNullOrWhiteSpace(prefix) || prefix.Contains('.'))
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync("Invalid tenant prefix.");
				return;
			}

			try
			{
				TenantResolution? resolved = await tenantResolver.ResolveByPrefixAsync(prefix, context.RequestAborted);
				if (resolved is null)
				{
					context.Response.StatusCode = StatusCodes.Status404NotFound;
					await context.Response.WriteAsync("Unknown tenant.");
					return;
				}

				tenantContext.IsLanding = false;
				tenantContext.Prefix = prefix;
				tenantContext.TenantId = resolved.TenantId;
				tenantContext.PublicName = resolved.Name;

				await this._next(context);
				return;
			}
			catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
			{
				// request aborted by client; no extra handling needed
				throw;
			}
		}

		// 3) Invalid host for this environment
		context.Response.StatusCode = StatusCodes.Status404NotFound;
		await context.Response.WriteAsync("Unknown tenant host.");
	}

	private bool IsBaseDomain(string host) =>
		string.Equals(host, this._options.BaseDomain, StringComparison.OrdinalIgnoreCase) ||
		string.Equals(host, $"www.{this._options.BaseDomain}", StringComparison.OrdinalIgnoreCase);
}