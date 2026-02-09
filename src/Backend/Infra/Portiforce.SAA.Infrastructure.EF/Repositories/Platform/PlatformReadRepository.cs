using Portiforce.SAA.Application.Interfaces.Persistence.Platform;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Platform.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Platform;

internal sealed class PlatformReadRepository : IPlatformReadRepository
{
	public Task<PlatformDetails?> GetByIdAsync(PlatformId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<PlatformListItem>> GetListByTenantIdAsync(TenantId tenantId, PageRequest pageRequest, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
