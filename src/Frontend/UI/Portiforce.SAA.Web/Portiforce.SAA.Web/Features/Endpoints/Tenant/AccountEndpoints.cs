using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Account;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class AccountEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup(ApiRoutes.Accounts.Root)
			.WithTags("Tenant Accounts")
			.RequireAuthorization(UiPolicies.TenantAdmin);

		group.MapGet(string.Empty, ListAccountsAsync)
			.WithName("ListTenantAccounts")
			.Produces<AccountListResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);

		group.MapGet("/{inviteId:guid}", GetAccountDetailsAsync)
			.WithName("GetTenantAccountDetails")
			.Produces<AccountDetailsResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);
	}

	private static async Task<Results<ForbidHttpResult, Ok<AccountListResponse>>> ListAccountsAsync(
		[AsParameters] GetAccountListQueryRequest request,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		AccountState? state = request.State?.ToBusiness();
		AccountTier? tier = request.Tier?.ToBusiness();

		var query = new GetAccountListQuery(
			currentUser.TenantId,
			request.Search,
			Role.None,
			AccountState.Unknown,
			AccountTier.None,
			new PageRequest(
				request.Page,
				request.PageSize));

		PagedResult<AccountListItem> result = await mediator.Send(query, ct);

		AccountListResponse response = result.MapToAccountList();
		return TypedResults.Ok(response);
	}

	private static async Task<Results<Ok<AccountDetailsResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> GetAccountDetailsAsync(
		Guid accountId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		var getDetailsCommand = new GetAccountDetailsQuery(new AccountId(accountId), currentUser.TenantId);
		TypedResult<AccountDetails> result = await mediator.Send(getDetailsCommand, ct);

		if (!result.IsSuccess || result.Value == null)
		{
			return TypedResults.NotFound();
		}

		AccountDetailsResponse inviteDetailsResponse = result.Value.MapToAccountDetails();
		return TypedResults.Ok(inviteDetailsResponse);
	}

}
