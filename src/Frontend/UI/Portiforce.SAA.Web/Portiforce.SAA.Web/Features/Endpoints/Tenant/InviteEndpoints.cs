using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;
using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;
using InviteChannel = Portiforce.SAA.Contracts.Enums.InviteChannel;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class InviteEndpoints : IEndpoint
{
	/*
		GET    /bff/invites
		GET    /bff/invites/{inviteId:guid}
		GET    /bff/invites/template
		POST   /bff/invites
		POST   /bff/invites/{inviteId:guid}/resend
		POST   /bff/invites/{inviteId:guid}/revoke
	 */

	private const int DefaultInviteLifetimeHours = 48;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup(ApiRoutes.Invites)
			.WithTags("Tenant Invites")
			.RequireAuthorization(UiPolicies.TenantAdmin)
			.AddEndpointFilter<ValidationFilter<CreateInviteRequest>>();

		group.MapGet(string.Empty, ListInvitesAsync)
			.WithName("ListTenantInvites")
			.Produces<InviteListResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);

		group.MapGet("/{inviteId:guid}", GetInviteDetailsAsync)
			.WithName("GetTenantInviteDetails")
			.Produces<InviteDetailsResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);

		group.MapPost(string.Empty, CreateInviteAsync)
			.WithName("CreateTenantInvite")
			.Produces<CreateInviteResponse>(StatusCodes.Status201Created)
			.ProducesValidationProblem()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status409Conflict);

		group.MapPost("/{inviteId:guid}/resend", ResendInviteAsync)
			.WithName("ResendTenantInvite")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);

		group.MapPost("/{inviteId:guid}/revoke", RevokeInviteAsync)
			.WithName("RevokeTenantInvite")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);
	}

	private static async Task<Results<Ok<InviteListResponse>, UnauthorizedHttpResult, ForbidHttpResult>> ListInvitesAsync(
		[AsParameters] GetInviteListQueryRequest request,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		InviteState? status = request.Status?.ToBusiness();
		Core.Identity.Enums.InviteChannel? channel = request.Channel?.ToBusiness();

		var query = new GetInviteListQuery(
			currentUser.TenantId,
			request.Search,
			status,
			channel,
			new PageRequest(
				request.Page,
				request.PageSize));

		PagedResult<InviteListItem> result = await mediator.Send(query, ct);

		InviteListResponse response = result.MapToInviteList();
		return TypedResults.Ok(response);
	}

	private static async Task<Results<Ok<InviteDetailsResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> GetInviteDetailsAsync(
		Guid inviteId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		var getDetailsCommand = new GetInviteDetailsQuery(currentUser.TenantId, inviteId);
		TypedResult<InviteDetails> result = await mediator.Send(getDetailsCommand, ct);

		if (!result.IsSuccess || result.Value == null) 
		{
			return TypedResults.NotFound();
		}

		InviteDetailsResponse inviteDetailsResponse = result.Value.MapToInviteDetails();
		return TypedResults.Ok(inviteDetailsResponse);
	}

	private static async Task<Results<Created<CreateInviteResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult, ForbidHttpResult>> CreateInviteAsync(
		[FromBody] CreateInviteRequest request,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		[FromServices] IClock clock,
		HttpContext httpContext,
		CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		Role? role = request.IntendedRole.ToBusiness();
		AccountTier? tier = request.IntendedTier.ToBusiness();

		if (role is null || tier is null)
		{
			return TypedResults.Problem(
				title: "Invalid invite payload",
				detail: "Unsupported role or tier.",
				statusCode: StatusCodes.Status400BadRequest);
		}

		InviteTarget inviteTarget;
		try
		{
			inviteTarget = request.Channel switch
			{
				InviteChannel.Email => InviteTarget.Email(request.TargetValue),
				InviteChannel.Telegram => InviteTarget.Telegram(request.TargetValue),
				InviteChannel.AppleId => InviteTarget.AppleId(request.TargetValue),
				_ => throw new ArgumentOutOfRangeException(nameof(request.Channel))
			};
		}
		catch (ArgumentException ex)
		{
			return TypedResults.Problem(
				title: "Invalid invite target",
				detail: ex.Message,
				statusCode: StatusCodes.Status400BadRequest);
		}

		DateTimeOffset now = clock.UtcNow;
		DateTimeOffset expiresAtUtc = now.AddHours(DefaultInviteLifetimeHours);

		var command = new CreateInviteCommand(
			TenantId: currentUser.TenantId,
			InviteTarget: inviteTarget,
			IntendedRole: role.Value,
			IntendedTier: tier.Value,
			InvitedByAccountId: currentUser.Id,
			CreatedAtUtc: now,
			ExpiredAtUtc: expiresAtUtc);

		TypedResult<CreateInviteResult> result = await mediator.Send(command, ct);

		if (!result.IsSuccess)
		{
			return TypedResults.Problem(
				title: "Invite creation failed",
				detail: result.Error?.Message,
				statusCode: MapToStatusCode(result));
		}

		var response = new CreateInviteResponse(
			InviteId: result.Value.InviteId,
			RawToken: result.Value.Token,
			ExpiresAtUtc: result.Value.ExpiresAtUtc);

		return TypedResults.Created($"/bff/invites/{result.Value.InviteId}", response);
	}

	private static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> ResendInviteAsync(
		Guid inviteId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		// todo : implement me
		//var resendInviteCommand = new ResendInviteCommand(currentUser.TenantId, inviteId);

		//TypedResult<ResendInviteResult> result = await mediator.Send(resendInviteCommand, ct);

		//if (!result.IsSuccess)
		//{
		//	return TypedResults.Problem(
		//		title: "Invite resend failed",
		//		detail: result.Error?.Message,
		//		statusCode: MapToStatusCode(result));
		//}

		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Resend invite endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> RevokeInviteAsync(
		Guid inviteId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		// todo: implement me
		//var revokeInviteCommand = new RevokeInviteCommand(currentUser.TenantId, inviteId);

		//TypedResult<RevokeInviteResult> result = await mediator.Send(revokeInviteCommand, ct);

		//if (!result.IsSuccess)
		//{
		//	return TypedResults.Problem(
		//		title: "Invite resend failed",
		//		detail: result.Error?.Message,
		//		statusCode: MapToStatusCode(result));
		//}

		return TypedResults.Problem(
			title: "Not Implemented",
			detail: "Revoke invite endpoint is not implemented yet.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static int MapToStatusCode<T>(TypedResult<T> result)
	{
		if (result.Error is null || string.IsNullOrEmpty(result.Error.Code))
		{
			return StatusCodes.Status400BadRequest;
		}

		var errorCode = result.Error.Code;

		if (errorCode.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
		{
			return StatusCodes.Status404NotFound;
		}

		if (errorCode.Contains("Conflict", StringComparison.OrdinalIgnoreCase))
		{
			return StatusCodes.Status409Conflict;
		}

		if (errorCode.Contains("Validation", StringComparison.OrdinalIgnoreCase))
		{
			return StatusCodes.Status400BadRequest;
		}

		// Default fallback
		return StatusCodes.Status400BadRequest;
	}
}