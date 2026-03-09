using Microsoft.AspNetCore.Antiforgery;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class TenantUiEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup(ApiRoutes.BffRoot).WithTags("Tenant UI API");

		group.MapGet(ApiRoutes.Tenant, (ITenantContext tenant) => Results.Ok(new { prefix = tenant.Prefix, isLanding = tenant.IsLanding }));

		// Used by the WASM client to call antiforgery-protected endpoints on the BFF.
		group.MapGet("/antiforgery", (HttpContext ctx, IAntiforgery antiforgery) =>
		{
			var tokens = antiforgery.GetAndStoreTokens(ctx);

			ctx.Response.Headers.CacheControl = "no-store";

			return Results.Ok(new
			{
				token = tokens.RequestToken
			});
		});
	}
}