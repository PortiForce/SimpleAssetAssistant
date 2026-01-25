using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Platform;

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
