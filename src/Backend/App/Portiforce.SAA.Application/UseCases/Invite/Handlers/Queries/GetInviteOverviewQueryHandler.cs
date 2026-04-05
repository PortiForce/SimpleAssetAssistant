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
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.NotFound("Invite not found.", request.RawToken));
		}

		TenantInvite? tenantInvite =
			await inviteReadRepository.GetByTenantAndTokenHashAsync(request.TenantId, tokenHash, ct);

		if (tenantInvite is null)
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.NotFound("Invite not found.", request.RawToken));
		}

		DateTimeOffset now = clock.UtcNow;
		if (tenantInvite.IsExpired(now))
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.Conflict("Invite expired."));
		}

		if (tenantInvite.State == InviteState.RevokedByTenant)
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.Conflict("Invite revoked."));
		}

		if (tenantInvite.State == InviteState.DeclinedByUser)
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.Conflict("Invite declined."));
		}

		if (tenantInvite.State == InviteState.Accepted)
		{
			return TypedResult<OverviewInviteDetails>.Fail(ResultError.Conflict("Invite already accepted."));
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
}