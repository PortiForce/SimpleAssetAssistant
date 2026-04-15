using System.Security.Claims;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Bff;

public sealed class MeEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet(ApiRoutes.Profile, (HttpContext ctx) =>
			{
				if (ctx.User.Identity?.IsAuthenticated != true)
				{
					return Results.Ok(new MeResponse(false, null, null, []));
				}

				string? userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
				string? email = ctx.User.FindFirstValue(ClaimTypes.Email);
				string[] roles = ctx.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

				return Results.Ok(new MeResponse(true, userId, email, roles));
			})
			.WithTags("BFF");
	}

	private sealed record MeResponse(bool IsAuthenticated, string? UserId, string? Email, string[] Roles);
}
