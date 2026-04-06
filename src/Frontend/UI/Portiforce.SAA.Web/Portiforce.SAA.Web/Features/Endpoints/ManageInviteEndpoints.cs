using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Exceptions;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Configuration;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;

namespace Portiforce.SAA.Web.Features.Endpoints;

public sealed class ManageInviteEndpoints : IEndpoint
{
	/*
        GET    /bff/invite/{inviteToken}
        POST   /bff/invite/{inviteToken}/decline
        POST   /bff/invite/{inviteToken}/accept
     */

	public const string GetPublicInviteOverviewEndpointName = "GetPublicInviteOverview";
	public const string DeclinePublicInviteEndpointName = "DeclinePublicInvite";
	public const string AcceptPublicInviteEndpointName = "AcceptPublicInviteStart";

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		RouteGroupBuilder group = app.MapGroup(ApiRoutes.PublicInviteRoutes.Root)
			.WithTags("Public")
			.AllowAnonymous();

		_ = group.MapGet("/{inviteToken}", GetInviteOverviewAsync)
			.WithName(GetPublicInviteOverviewEndpointName)
			.Produces<OverviewInviteDetailsResponse>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status410Gone);

		_ = group.MapPost("/{inviteToken}/decline", DeclineInviteAsync)
			.WithName(DeclinePublicInviteEndpointName)
			.Produces<DeclineInviteResponse>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status410Gone);

		_ = group.MapPost("/{inviteToken}/accept", StartAcceptInviteAsync)
			.WithName(AcceptPublicInviteEndpointName)
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status410Gone);
	}

	private static async Task<
			Results<
				Ok<OverviewInviteDetailsResponse>,
				BadRequest<ApiProblemDetails>,
				NotFound,
				Conflict<ApiProblemDetails>>>
		GetInviteOverviewAsync(
			[FromRoute] string inviteToken,
			[FromServices] ITenantContext tenantContext,
			[FromServices] IMediator mediator,
			[FromServices] LinkGenerator linkGenerator,
			HttpContext httpContext,
			CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(inviteToken))
		{
			return TypedResults.BadRequest(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status400BadRequest,
					"Invalid invite token.",
					"Invite token is required."));
		}

		if (!tenantContext.TenantId.HasValue || tenantContext.TenantId.Value == Guid.Empty)
		{
			return TypedResults.Conflict(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status409Conflict,
					"Tenant context is missing.",
					"The invite cannot be processed because tenant context was not resolved."));
		}

		TenantId tenantId = new(tenantContext.TenantId.Value);

		GetInviteOverviewQuery query = new(tenantId, inviteToken);
		TypedResult<OverviewInviteDetails> result = await mediator.Send(query, ct);

		if (!result.IsSuccess || result.Value is null)
		{
			return TypedResults.NotFound();
		}

		OverviewInviteDetailsResponse response = result.Value.MapToOverviewInviteDetails(
			result.Value.CanAccept,
			result.Value.CanDecline);

		return TypedResults.Ok(response);
	}

	private static async Task<
			Results<
				Ok<DeclineInviteResponse>,
				BadRequest<ApiProblemDetails>,
				NotFound,
				Conflict<ApiProblemDetails>>>
		DeclineInviteAsync(
			[FromRoute] string inviteToken,
			[FromServices] ITenantContext tenantContext,
			[FromServices] IMediator mediator,
			CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(inviteToken))
		{
			return TypedResults.BadRequest(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status400BadRequest,
					"Invalid invite token.",
					"Invite token is required."));
		}

		if (!tenantContext.TenantId.HasValue || tenantContext.TenantId.Value == Guid.Empty)
		{
			return TypedResults.Conflict(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status409Conflict,
					"Tenant context is missing.",
					"The invite cannot be processed because tenant context was not resolved."));
		}

		TenantId tenantId = new(tenantContext.TenantId.Value);

		DeclineInviteCommand command = new(tenantId, inviteToken);
		TypedResult<DeclineInviteResult> result = await mediator.Send(command, ct);

		if (!result.IsSuccess || result.Value is null)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(result.Value.MapToResponse());
	}

	private static async Task<Results<
			Ok<StartAcceptInviteResponse>,
			BadRequest<ApiProblemDetails>,
			NotFound,
			Conflict<ApiProblemDetails>>>
		StartAcceptInviteAsync(
			[FromRoute] string inviteToken,
			[FromServices] ITenantContext tenantContext,
			[FromServices] IMediator mediator,
			LinkGenerator linkGenerator,
			HttpContext httpContext,
			CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(inviteToken))
		{
			return TypedResults.BadRequest(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status400BadRequest,
					"Invalid invite token.",
					"Invite token is required."));
		}

		if (!tenantContext.TenantId.HasValue || tenantContext.TenantId.Value == Guid.Empty)
		{
			return TypedResults.Conflict(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status409Conflict,
					"Tenant context is missing.",
					"The invite cannot be processed because tenant context was not resolved."));
		}

		TenantId tenantId = new(tenantContext.TenantId.Value);

		GetInviteOverviewQuery query = new(tenantId, inviteToken);
		TypedResult<OverviewInviteDetails> typedInviteDetailsResult = await mediator.Send(query, ct);

		if (!typedInviteDetailsResult.IsSuccess || typedInviteDetailsResult.Value is null)
		{
			return TypedResults.NotFound();
		}

		OverviewInviteDetails invite = typedInviteDetailsResult.Value;

		if (!invite.CanAccept)
		{
			return TypedResults.Conflict(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status409Conflict,
					"Invite cannot be accepted.",
					"The invite cannot be processed because it can no longer be accepted."));
		}

		string endpointName = ResolveAcceptAuthEndpointName(invite);

		string acceptInviteEndpoint = linkGenerator.GetPathByName(
										  httpContext,
										  endpointName,
										  new { inviteToken })
									  ?? throw new InvalidOperationException(
										  $"Could not generate path for endpoint '{endpointName}'.");

		return TypedResults.Ok(new StartAcceptInviteResponse(acceptInviteEndpoint));
	}

	private static string ResolveAcceptAuthEndpointName(OverviewInviteDetails invite) =>
		invite switch
		{
			{ InviteChannel: InviteChannel.Email } => AuthEndpointNames.GoogleLogin,
			{ InviteChannel: InviteChannel.Telegram } => AuthEndpointNames.TelegramLogin,
			{
				InviteChannel: InviteChannel.AppleAccount,
				InviteTargetKind: InviteTargetKind.Email
			} => AuthEndpointNames.AppleLogin,
			{ InviteChannel: InviteChannel.AppleAccount } => AuthEndpointNames.ApplePhoneLogin,
			_ => throw new NotSupportedException($"Provided use case is not supported. Channel: {invite.InviteChannel}")
		};

	private static string GetDeclineEndpoint(
		LinkGenerator linkGenerator,
		HttpContext httpContext,
		string inviteToken) =>
		linkGenerator.GetPathByName(
			httpContext,
			DeclinePublicInviteEndpointName,
			new { inviteToken })
		?? throw new InvalidOperationException("Could not generate decline invite endpoint.");

	private static string GetAcceptEndpoint(
		LinkGenerator linkGenerator,
		HttpContext httpContext,
		string inviteToken) =>
		linkGenerator.GetPathByName(
			httpContext,
			AcceptPublicInviteEndpointName,
			new { inviteToken })
		?? throw new InvalidOperationException("Could not generate accept invite endpoint.");
}