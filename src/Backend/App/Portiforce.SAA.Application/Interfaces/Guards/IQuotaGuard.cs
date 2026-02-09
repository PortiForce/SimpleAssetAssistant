using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Guards;

public interface IQuotaGuard
{
	Task EnsureCanCreateUserAsync(TenantId tenantId, CancellationToken ct);
	Task EnsureCanAddPlatformAsync(TenantId tenantId, CancellationToken ct);
	Task EnsureCanUseAssetsAsync(TenantId tenantId, int distinctAssetsAfter, CancellationToken ct);
}
