using Microsoft.AspNetCore.Http.HttpResults;

using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class AccountEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/tenant/account")
			.WithTags("Tenant Account")
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
}
