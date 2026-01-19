using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
	
public interface IAssetReadRepository : IReadRepository<AssetDetails, AssetId>
{
	Task<bool> ExistsByCodeAsync(AssetCode code, CancellationToken ct);

	/// <summary>
	/// Return list of assets that are allowed/accepted by Tenant entity
	/// </summary>
	/// <param name="tenantId"></param>
	/// <param name="pageRequest"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	Task<PagedResult<AssetListItem>> GetListByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct);

	/// <summary>
	/// Return list of asset defined by Asset Ids
	/// </summary>
	/// <param name="assetIds">list of asset Ids to fetch</param>
	/// <param name="pageRequest"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	Task<PagedResult<AssetListItem>> GetListByAssetIdsAsync(
		IReadOnlyCollection<AssetId> assetIds,
		PageRequest pageRequest,
		CancellationToken ct);

	Task<PagedResult<AssetListItem>> GetListByAccountIdAsync(
		AccountId accountId,
		PageRequest pageRequest,
		CancellationToken ct);

	Task<PagedResult<AssetListItem>> GetListByAccountIdAndPlatformIdAsync(
		AccountId accountId,
		PlatformId platformId,
		PageRequest pageRequest,
		CancellationToken ct);
}
