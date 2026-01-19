using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;

public interface IPlatformReadRepository : IReadRepository<PlatformDetails, PlatformId>
{
	Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
	
	Task<PagedResult<PlatformListItem>> GetListByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct);
}
