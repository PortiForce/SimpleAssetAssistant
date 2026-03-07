using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenants;

public sealed class TenantUiEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/ui-api").WithTags("UI API");

		group.MapGet("/tenant", (ITenantContext tenant) =>
		{
			return Results.Ok(new { prefix = tenant.Prefix, isLanding = tenant.IsLanding });
		});
	}
}