using Portiforce.SimpleAssetAssistant.Application.Entitlements;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Tenant;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Resolvers;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Tenant;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.SharedFlow.Services;

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

