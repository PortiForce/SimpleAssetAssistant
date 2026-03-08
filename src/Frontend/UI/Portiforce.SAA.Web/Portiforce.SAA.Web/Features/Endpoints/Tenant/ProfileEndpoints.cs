using Microsoft.AspNetCore.Http.HttpResults;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class ProfileEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup(ApiRoutes.Ptofile)
			.WithTags("Profile")
			.RequireAuthorization(UiPolicies.TenantUser);

		// Dashboard Placeholder
		group.MapGet("/dashboard", GetDashboardAsync)
			.WithName("GetTenantDashboard")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status501NotImplemented);

		// Portfolio Placeholder
		group.MapGet("/portfolio", GetPortfolioAsync)
			.WithName("GetTenantPortfolio")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status501NotImplemented);

		// Assets Placeholder
		group.MapGet("/assets", GetAssetsAsync)
			.WithName("GetAssets")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<Ok, ProblemHttpResult>> GetDashboardAsync(CancellationToken ct)
	{
		// Safe placeholder that won't crash the app on startup
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Dashboard endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<Ok, ProblemHttpResult>> GetPortfolioAsync(CancellationToken ct)
	{
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Portfolio endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<Ok, ProblemHttpResult>> GetAssetsAsync(CancellationToken ct)
	{
		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Assets endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}
}
