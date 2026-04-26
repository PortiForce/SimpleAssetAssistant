using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Models.Client;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Security;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class TenantUiEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		RouteGroupBuilder group = app.MapGroup(ApiRoutes.BffRoot).WithTags("Tenant UI API");

		_ = group.MapGet(
			"/tenant",
			(ITenantContext tenant, IOptions<TenancyOptions> options) => Results.Ok(new TenantInfo
			{
				Prefix = tenant.Prefix,
				IsLanding = tenant.IsLanding,
				BaseDomain = options.Value.BaseDomain
			}));

		// Used by the WASM client to call antiforgery-protected endpoints on the BFF.
		_ = group.MapGet("/antiforgery", (HttpContext ctx, IAntiforgery antiforgery) =>
		{
			AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(ctx);

			ctx.Response.Headers.CacheControl = "no-store";

			return Results.Ok(new
			{
				token = tokens.RequestToken
			});
		});
	}
}
