using Portiforce.SAA.Application.Entitlements;
using Portiforce.SAA.Application.Interfaces.Models.Tenant;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Resolvers;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Application.UseCases.SharedFlow.Services;

internal sealed class TenantLimitsService(
	ITenantEntitlementsResolver tenantEntitlementsResolver,
	ITenantReadRepository tenantReadRepository,
	IAccountReadRepository accountReadRepository) : ITenantLimitsService
{
	public async Task EnsureTenantCanInviteOrActivateUserAsync(ITenantInfo tenantInfo, CancellationToken ct)
	{
		TenantEntitlements tenantEntitlements = tenantEntitlementsResolver.Resolve(tenantInfo.Plan);

		int maxAllowedActiveUsers = tenantEntitlements.MaxUsers;

		int currentActiveUsers = await accountReadRepository.GetActiveUserCountAsync(tenantInfo.Id, ct);

		if (currentActiveUsers >= maxAllowedActiveUsers)
		{
			throw new DomainValidationException(
				$"Tenant '{tenantInfo.Code}' has reached the maximum of {maxAllowedActiveUsers} active users. Please contact your admin to upgrade.");
		}
	}
}

