using Portiforce.SAA.Application.Interfaces.Persistence.Asset;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Asset.Projections;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Asset;

internal sealed class AssetReadRepository : IAssetReadRepository
{
	public Task<AssetDetails?> GetByIdAsync(AssetId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<bool> ExistsByCodeAsync(AssetCode code, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<AssetListItem>> GetListByTenantIdAsync(TenantId tenantId, PageRequest pageRequest, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<IReadOnlyList<AssetListItem>> GetListByAssetIdsAsync(IReadOnlyCollection<AssetId> assetIds, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<AssetListItem>> GetListByAccountIdAsync(AccountId accountId, PageRequest pageRequest, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<AssetListItem>> GetListByAccountIdAndPlatformIdAsync(AccountId accountId, PlatformId platformId, PageRequest pageRequest,
		CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
