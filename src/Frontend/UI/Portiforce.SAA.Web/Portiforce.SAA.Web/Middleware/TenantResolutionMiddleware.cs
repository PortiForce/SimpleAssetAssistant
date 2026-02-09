namespace Portiforce.SAA.Web.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next, IConfiguration config)
{
	// Defined in appsettings: "BaseDomain": "dev.localhost"
	private readonly string _baseDomain = config["Tenancy:BaseDomain"] ?? "dev.localhost";

	public async Task InvokeAsync(HttpContext context)
	{
		var host = context.Request.Host.Host;

		// 1. Check for Landing Page (Root Domain)
		if (host.Equals(_baseDomain, StringComparison.OrdinalIgnoreCase) ||
		    host.Equals($"www.{_baseDomain}", StringComparison.OrdinalIgnoreCase))
		{
			// No tenant context. Proceed as "Landing Page".
			await next(context);
			return;
		}

		// 2. Check for Subdomain
		if (host.EndsWith($".{_baseDomain}", StringComparison.OrdinalIgnoreCase))
		{
			// Extract "app" from "app.dev.localhost"
			var prefix = host.Substring(0, host.Length - _baseDomain.Length - 1);

			// TODO: Validate against DB cache here (Phase 2)
			// For now, valid format = valid tenant
			context.Items["TenantPrefix"] = prefix;

			await next(context);
			return;
		}

		// 3. Invalid Host (e.g. random IP access)
		context.Response.StatusCode = 404;
		await context.Response.WriteAsync("Unknown Tenant Host");
	}
}
