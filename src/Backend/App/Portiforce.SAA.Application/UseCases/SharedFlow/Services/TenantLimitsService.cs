using Portiforce.SAA.Application.Entitlements;
using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Models.Tenant;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Resolvers;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.SharedFlow.Services;

internal sealed class TenantLimitsService(
	ITenantEntitlementsResolver tenantEntitlementsResolver,
	ITenantReadRepository tenantReadRepository,
	IAccountReadRepository accountReadRepository,
	IInviteReadRepository inviteReadRepository) : ITenantLimitsService
{
	public async Task<Result> EnsureTenantCanInviteOrActivateAccountAsync(TenantId tenantId, CancellationToken ct)
	{
		if (tenantId == TenantId.Empty)
		{
			return Result.Fail(ResultError.Validation("Tenant Id:is not defined"));
		}

		TenantSummary? tenantSummary = await tenantReadRepository.GetSummaryByIdAsync(tenantId, ct);
		if (tenantSummary is null)
		{
			return Result.Fail(ResultError.Validation($"Tenant with Id: {tenantId} is not found"));
		}

		if (tenantSummary.State != TenantState.Active)
		{
			return Result.Fail(ResultError.Validation($"Tenant with Id: {tenantId} is not active"));
		}

		return await this.EnsureTenantInvitesAndAccountLimitsAsync(tenantSummary, ct);
	}

	public async Task<Result> EnsureTenantInvitesAndAccountLimitsAsync(ITenantInfo tenantInfo, CancellationToken ct)
	{
		TenantEntitlements tenantEntitlements = tenantEntitlementsResolver.Resolve(tenantInfo.Plan);

		int maxAllowedActiveUsers = tenantEntitlements.MaxActiveUsers;
		int currentActiveUsers = await accountReadRepository.GetActiveUserCountAsync(tenantInfo.Id, ct);

		if (currentActiveUsers >= maxAllowedActiveUsers)
		{
			return Result.Fail(
				ResultError.Validation(
					$"Tenant '{tenantInfo.Code}' has reached the maximum of {maxAllowedActiveUsers} active users. Please contact your admin to upgrade."));
		}

		int maxPendingInvites = tenantEntitlements.MaxPendingInvites;
		int currentPendingInvites = await inviteReadRepository.GetPendingInviteCountAsync(tenantInfo.Id, ct);

		if (currentPendingInvites >= maxPendingInvites)
		{
			return Result.Fail(
				ResultError.Validation(
					$"Tenant '{tenantInfo.Code}' has reached the maximum of {maxPendingInvites} pending invites. Please contact your admin to upgrade."));
		}

		return Result.Ok();
	}
}