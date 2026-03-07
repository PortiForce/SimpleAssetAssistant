using Microsoft.AspNetCore.Http.HttpResults;

using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class AdminEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/tenant/admin")
			.WithTags("Tenant Admin")
			.RequireAuthorization(UiPolicies.TenantAdmin);

		// Tenant Settings Placeholder
		group.MapGet("/settings", GetSettingsAsync)
			.WithName("GetTenantSettings")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status501NotImplemented);

		// User Management Placeholder
		group.MapGet("/users", GetUsersAsync)
			.WithName("GetTenantUsers")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<Ok, ProblemHttpResult>> GetSettingsAsync(CancellationToken ct)
	{
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Tenant settings endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<Ok, ProblemHttpResult>> GetUsersAsync(CancellationToken ct)
	{
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Tenant users endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}
}
