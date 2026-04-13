using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Flow.Mappers;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteOverviewQueryHandler(
	IHashingService hashing,
	ITenantLimitsService tenantLimitsService,
	IClock clock,
	IInviteReadRepository inviteReadRepository)
	: IRequestHandler<GetInviteOverviewQuery, TypedResult<OverviewInviteDetails>>
{
	public async ValueTask<TypedResult<OverviewInviteDetails>> Handle(
		GetInviteOverviewQuery request,
		CancellationToken ct)
	{
		byte[] tokenHash;
		try
		{
			tokenHash = hashing.HashInviteToken(request.RawToken);
		}
		catch (ArgumentException)
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.NotFound("Invite", request.RawToken));
		}

		TenantInvite? tenantInvite =
			await inviteReadRepository.GetByTenantAndTokenHashAsync(request.TenantId, tokenHash, ct);

		if (tenantInvite is null)
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.NotFound("Invite", request.RawToken));
		}

		DateTimeOffset now = clock.UtcNow;

		if (request.CurrentUser.IsAuthenticated &&
			!CanAuthenticatedUserSeeInvite(request.CurrentUser.Id, tenantInvite))
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.NotFound("Invite", request.RawToken));
		}

		if (tenantInvite.State == InviteState.Accepted)
		{
			OverviewInviteDetails acceptedInviteDetails = InviteProjectionMapper.ToOverviewDetails(tenantInvite, now);
			return TypedResult<OverviewInviteDetails>.Ok(acceptedInviteDetails);
		}

		if (tenantInvite.State == InviteState.RevokedByTenant)
		{
			OverviewInviteDetails revokedInviteDetails = InviteProjectionMapper.ToOverviewDetails(tenantInvite, now);
			return TypedResult<OverviewInviteDetails>.Ok(revokedInviteDetails);
		}

		if (tenantInvite.State == InviteState.DeclinedByUser)
		{
			OverviewInviteDetails declinedInviteDetails = InviteProjectionMapper.ToOverviewDetails(tenantInvite, now);
			return TypedResult<OverviewInviteDetails>.Ok(declinedInviteDetails);
		}

		if (tenantInvite.IsExpired(now))
		{
			OverviewInviteDetails expiredInviteDetails = InviteProjectionMapper.ToOverviewDetails(tenantInvite, now);
			return TypedResult<OverviewInviteDetails>.Ok(expiredInviteDetails);
		}

		FlowResult.Result limitChecksResult =
			await tenantLimitsService.EnsureTenantCanInviteOrActivateAccountAsync(request.TenantId, ct);

		if (!limitChecksResult.IsSuccess)
		{
			return TypedResult<OverviewInviteDetails>.Fail(
				limitChecksResult.Error ?? ResultError.Validation("Tenant has no longer capability to accept invites"));
		}

		OverviewInviteDetails inviteDetails = InviteProjectionMapper.ToOverviewDetails(tenantInvite, now);

		return TypedResult<OverviewInviteDetails>.Ok(inviteDetails);
	}

	private static bool CanAuthenticatedUserSeeInvite(AccountId currentAccountId, TenantInvite tenantInvite)
	{
		if (currentAccountId == AccountId.Empty)
		{
			return false;
		}

		if (tenantInvite.State != InviteState.Accepted)
		{
			// it should not be possible for authenticated user to see any invite details except accepted ones, as we don't want to leak any information about the invite existence or state
			return false;
		}

		// check relation of the existing account with invite's details
		return tenantInvite.AcceptedAccountId.HasValue &&
			   tenantInvite.AcceptedAccountId.Value == currentAccountId;
	}
}