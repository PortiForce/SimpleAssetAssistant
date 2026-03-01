using Portiforce.SAA.Web.Services;

namespace Portiforce.SAA.Web.Filters;

public sealed class TenantResolutionFilter : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(
		EndpointFilterInvocationContext context,
		EndpointFilterDelegate next)
	{
		if (!context.HttpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantValues) ||
		    !Guid.TryParse(tenantValues.FirstOrDefault(), out var tenantId))
		{
			return TypedResults.BadRequest(new { Error = "Missing or invalid X-Tenant-Id header." });
		}

		var tenantContext = context.HttpContext.RequestServices.GetRequiredService<TenantContext>();
		tenantContext.TenantId = tenantId;

		return await next(context);
	}
}
