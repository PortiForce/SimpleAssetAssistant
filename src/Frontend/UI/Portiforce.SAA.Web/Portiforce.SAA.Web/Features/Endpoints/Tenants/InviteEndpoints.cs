using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Enums;
using Portiforce.SAA.Contracts.Models.Invite;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;

using InviteChannel = Portiforce.SAA.Contracts.Enums.InviteChannel;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenants;

public sealed class InviteEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		// 1. Create a secure group for all Tenant Admin operations
		var group = app.MapGroup("/api/v1/tenant/invites")
			.WithTags("Tenant Invites")
			.RequireAuthorization(UiPolicies.TenantAdmin);

		// 2. Map the POST endpoint
		group.MapPost("/", CreateInviteAsync)
			 .WithName("CreateTenantInvite")
			 .Produces<Guid>(StatusCodes.Status200OK)
			 .ProducesProblem(StatusCodes.Status400BadRequest)
			 .ProducesProblem(StatusCodes.Status409Conflict);
	}

	private static async Task<IResult> CreateInviteAsync(
		[FromBody] CreateInviteRequest request,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		HttpContext context,
		CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated)
		{
			return TypedResults.Unauthorized();
		}

		TenantId tenantId = currentUser.TenantId;
		if (tenantId == TenantId.Empty)
		{
			return TypedResults.BadRequest("No active tenant context found.");
		}

		AccountId currentAccountId = currentUser.Id;
		

		InviteTarget inviteTarget = request.Channel switch
		{
			InviteChannel.Email => InviteTarget.Email(request.Value),
			InviteChannel.Telegram => InviteTarget.Telegram(request.Value),
			InviteChannel.AppleId => InviteTarget.AppleId(request.Value),
			_ => throw new ArgumentOutOfRangeException(nameof(request.Channel), "Unsupported invite channel")
		};

		Role role = FromInviteRole(request.IntendedRole);
		if (role == Role.None)
		{
			return TypedResults.BadRequest("Invalid intended role.");
		}

		var tier = FromInviteTier(request.IntendedTier);
		if (tier == AccountTier.None)
		{
			return TypedResults.BadRequest("Invalid intended tier.");
		}

		var command = new CreateInviteCommand(
			TenantId: tenantId,
			InviteTarget: inviteTarget,
			IntendedRole: role,
			IntendedTier: tier,
			InvitedByAccountId: currentAccountId,
			CreatedAtUtc: DateTimeOffset.UtcNow,
			ExpiredAtUtc: DateTimeOffset.UtcNow.AddHours(48)
		);

		TypedResult<CreateInviteResult> result = await mediator.Send(command, ct);

		return result.IsSuccess
			? TypedResults.Ok(result.Value.InviteId)
			: TypedResults.BadRequest(result.Error.Message);
	}

	private static Role FromInviteRole(InviteTenantRole inviteRole)
	{
		return inviteRole switch
		{
			InviteTenantRole.TenantUser => Role.TenantUser,
			InviteTenantRole.TenantAdmin => Role.TenantAdmin,
			_ => Role.None
		};
	}

	private static AccountTier FromInviteTier(InviteAccountTier inviteTier)
	{
		return inviteTier switch
		{
			InviteAccountTier.Observer => AccountTier.Observer,
			InviteAccountTier.Investor => AccountTier.Investor,
			InviteAccountTier.Strategist => AccountTier.Strategist,
			_ => AccountTier.None
		};
	}
}