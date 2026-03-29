using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;

namespace Portiforce.SAA.Web.Features.Endpoints;

public sealed class PublicEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		RouteGroupBuilder group = app.MapGroup(ApiRoutes.Public)
			.WithTags("Public")
			.AllowAnonymous();

		_ = group.MapPost("/invites/{inviteToken}/decline", DeclineInviteAsync)
			.WithName("DeclinePublicInvite")
			.Produces<DeclineInviteResponse>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status410Gone);
	}

	private static async Task<Results<
			Ok<DeclineInviteResponse>,
			BadRequest<ProblemDetails>,
			NotFound,
			Conflict<ProblemDetails>>>
		DeclineInviteAsync(
			[FromRoute] string inviteToken,
			[FromServices] ITenantContext tenantContext,
			[FromServices] IMediator mediator,
			CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(inviteToken))
		{
			return TypedResults.BadRequest(
				new ProblemDetails
				{
					Title = "Invalid invite token.",
					Detail = "Invite token is required.",
					Status = StatusCodes.Status400BadRequest
				});
		}

		if (!tenantContext.TenantId.HasValue || tenantContext.TenantId.Value == Guid.Empty)
		{
			return TypedResults.Conflict(
				new ProblemDetails
				{
					Title = "Tenant context is missing.",
					Detail = "The invite cannot be processed because tenant context was not resolved.",
					Status = StatusCodes.Status409Conflict
				});
		}

		TenantId tenantId = new(tenantContext.TenantId.Value);

		DeclineInviteCommand command = new(tenantId, inviteToken);
		TypedResult<DeclineInviteResult> result = await mediator.Send(command, ct);

		if (!result.IsSuccess || result.Value is null)
		{
			return TypedResults.NotFound();
		}

		DeclineInviteResponse response = result.Value.MapToResponse();
		return TypedResults.Ok(response);
	}
}