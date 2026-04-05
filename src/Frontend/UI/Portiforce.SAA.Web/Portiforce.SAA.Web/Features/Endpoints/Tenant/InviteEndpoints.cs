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
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Enums;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.Models.Client.Invite.Summary;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;

using InviteChannel = Portiforce.SAA.Contracts.Enums.InviteChannel;
using InviteSummaryRange = Portiforce.SAA.Contracts.Enums.InviteSummaryRange;
using InviteTargetKind = Portiforce.SAA.Contracts.Enums.InviteTargetKind;
using InviteTrendBucket = Portiforce.SAA.Contracts.Enums.InviteTrendBucket;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenant;

public sealed class InviteEndpoints : IEndpoint
{
	/*
        GET    /bff/admin-invite
        GET    /bff/admin-invite/summary
        GET    /bff/admin-invite/{inviteId:guid}
        GET    /bff/admin-invite/template
        POST   /bff/admin-invite
        POST   /bff/admin-invite/{inviteId:guid}/resend
        POST   /bff/admin-invite/{inviteId:guid}/revoke
     */

	private const string ListTenantInvitesEndpointName = "ListTenantInvites";
	private const string GetTenantInviteSummaryEndpointName = "GetTenantInviteSummary";
	private const string GetTenantInviteDetailsEndpointName = "GetTenantInviteDetails";
	private const string CreateTenantInviteEndpointName = "CreateTenantInvite";
	private const string ResendTenantInviteEndpointName = "ResendTenantInvite";
	private const string RevokeTenantInviteEndpointName = "RevokeTenantInvite";

	private const int DefaultInviteLifetimeHours = 48;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		RouteGroupBuilder group = app.MapGroup(ApiRoutes.AdminInviteRoutes.Root)
			.WithTags("Tenant Invites")
			.RequireAuthorization(UiPolicies.TenantAdmin)
			.AddEndpointFilter<ValidationFilter<CreateInviteRequest>>();

		_ = group.MapGet(string.Empty, ListInvitesAsync)
			.WithName(ListTenantInvitesEndpointName)
			.Produces<InviteListResponse>()
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);

		_ = group.MapGet("/summary", GetInviteSummaryAsync)
			.WithName(GetTenantInviteSummaryEndpointName)
			.Produces<InviteSummaryResponse>()
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);

		_ = group.MapGet("/{inviteId:guid}", GetInviteDetailsAsync)
			.WithName(GetTenantInviteDetailsEndpointName)
			.Produces<AdminInviteDetailsResponse>()
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);

		_ = group.MapPost("/new", CreateInviteAsync)
			.WithName(CreateTenantInviteEndpointName)
			.Produces<CreateInviteResponse>(StatusCodes.Status201Created)
			.ProducesValidationProblem()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status422UnprocessableEntity);

		_ = group.MapPost("/{inviteId:guid}/resend", ResendInviteAsync)
			.WithName(ResendTenantInviteEndpointName)
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);

		_ = group.MapPost("/{inviteId:guid}/revoke", RevokeInviteAsync)
			.WithName(RevokeTenantInviteEndpointName)
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);
	}

	private static async Task<Results<Ok<InviteListResponse>, UnauthorizedHttpResult, ForbidHttpResult>>
		ListInvitesAsync(
			[FromQuery] string? search,
			[FromQuery] InviteStatus[]? statuses,
			[FromQuery] InviteChannel[]? channels,
			[FromQuery] int page,
			[FromQuery] int pageSize,
			[FromQuery] bool? hasAccount,
			[FromServices] IMediator mediator,
			[FromServices] ICurrentUser currentUser,
			CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		HashSet<InviteState> statusList = statuses?.ToBusinessSet();
		HashSet<Core.Identity.Enums.InviteChannel> channelList = channels?.ToBusinessSet();

		GetInviteListQuery query = new(
			currentUser.TenantId,
			search,
			statusList,
			channelList,
			hasAccount,
			new PageRequest(
				page,
				pageSize));

		PagedResult<InviteListItem> result = await mediator.Send(query, ct);

		InviteListResponse response = result.MapToInviteList();
		return TypedResults.Ok(response);
	}

	private static async Task<Results<Ok<InviteSummaryResponse>, UnauthorizedHttpResult, ForbidHttpResult>>
		GetInviteSummaryAsync(
			[FromQuery] InviteStatus[]? statuses,
			[FromQuery] InviteChannel[]? channels,
			[FromQuery] DateTime? fromDate,
			[FromQuery] DateTime? toDate,
			[FromQuery] bool? hasAccount,
			[FromQuery] bool? includeRevoked,
			[FromQuery] InviteSummaryRange range,
			[FromQuery] InviteTrendBucket trendBucket,
			[FromServices] IMediator mediator,
			[FromServices] ICurrentUser currentUser,
			CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		HashSet<InviteState> statusList = statuses?.ToBusinessSet();
		HashSet<Core.Identity.Enums.InviteChannel> channelList = channels?.ToBusinessSet();

		GetInviteSummaryQuery query = new(
			currentUser.TenantId,
			statusList,
			channelList,
			fromDate,
			toDate,
			hasAccount,
			includeRevoked,
			range.ToBusiness(),
			trendBucket.ToBusiness());

		InviteSummary result = await mediator.Send(query, ct);

		InviteSummaryResponse response = result.MapToInviteSummary();
		return TypedResults.Ok(response);
	}

	private static async
		Task<Results<Ok<AdminInviteDetailsResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>>
		GetInviteDetailsAsync(
			Guid inviteId,
			[FromServices] IMediator mediator,
			[FromServices] ICurrentUser currentUser,
			CancellationToken ct)
	{
		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		GetInviteDetailsQuery getDetailsCommand = new(currentUser.TenantId, inviteId);
		TypedResult<AdminInviteDetails> result = await mediator.Send(getDetailsCommand, ct);

		if (!result.IsSuccess || result.Value == null)
		{
			return TypedResults.NotFound();
		}

		AdminInviteDetailsResponse inviteDetailsResponse = result.Value.MapToInviteDetails();
		return TypedResults.Ok(inviteDetailsResponse);
	}

	private static async
		Task<Results<Created<CreateInviteResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult,
			ForbidHttpResult>> CreateInviteAsync(
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
			inviteTarget = request.TargetKind switch
			{
				InviteTargetKind.Phone => InviteTarget.ApplePhone(request.TargetValue),
				InviteTargetKind.Email => request.Channel == InviteChannel.AppleAccount
					? InviteTarget.AppleEmail(request.TargetValue)
					: InviteTarget.Email(request.TargetValue),
				InviteTargetKind.TelegramUserName => InviteTarget.TelegramUserName(request.TargetValue),
				_ => throw new ArgumentOutOfRangeException(nameof(request.TargetKind))
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

		CreateInviteCommand command = new(
			currentUser.TenantId,
			inviteTarget,
			role.Value,
			tier.Value,
			currentUser.Id,
			now,
			expiresAtUtc);

		TypedResult<CreateInviteResult> result = await mediator.Send(command, ct);

		if (!result.IsSuccess)
		{
			return TypedResults.Problem(
				title: "Invite creation failed",
				detail: result.Error?.Message,
				statusCode: MapToStatusCode(result));
		}

		CreateInviteResponse response = new(
			result.Value.InviteId,
			result.Value.Token,
			result.Value.ExpiresAtUtc);

		return TypedResults.Created($"/bff/invites/{result.Value.InviteId}", response);
	}

	private static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>>
		ResendInviteAsync(
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

	private static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>>
		RevokeInviteAsync(
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

		string errorCode = result.Error.Code;

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