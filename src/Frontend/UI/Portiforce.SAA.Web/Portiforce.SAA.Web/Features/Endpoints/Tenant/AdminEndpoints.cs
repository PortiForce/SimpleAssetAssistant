using Microsoft.AspNetCore.Http.HttpResults;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class AdminEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup(ApiRoutes.Admin)
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

		// Platform setting Management Placeholder
		group.MapGet("/platforms", GetPlatformsAsync)
			.WithName("GetPlatformSettings")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status501NotImplemented);

		// Assets setting Management Placeholder
		group.MapGet("/assets", GetAssetsAsync)
			.WithName("GetAssetSettings")
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

	private static async Task<Results<Ok, ProblemHttpResult>> GetPlatformsAsync(CancellationToken ct)
	{
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Platform management endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<Ok, ProblemHttpResult>> GetAssetsAsync(CancellationToken ct)
	{
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Asset management endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}
}
