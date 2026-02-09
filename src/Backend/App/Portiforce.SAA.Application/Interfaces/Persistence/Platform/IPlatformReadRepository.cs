using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Platform.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Platform;

public interface IPlatformReadRepository : IReadRepository<PlatformDetails, PlatformId>
{
	Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
	
	Task<PagedResult<PlatformListItem>> GetListByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct);
}
