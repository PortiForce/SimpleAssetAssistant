using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
	
public interface IAssetRepository : IRepository<Portiforce.SimpleAssetAssistant.Core.Assets.Models.Asset, AssetId>
{
	public Task UpdateAsync(Core.Assets.Models.Asset asset, CancellationToken ct);

	/// <summary>
	/// Return list of assets that are allowed/accepted by Tenant entity
	/// </summary>
	/// <param name="tenantId"></param>
	/// <param name="pageRequest"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	public Task<PagedResult<Core.Assets.Models.Asset>> GetListByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct);

	public Task<PagedResult<Core.Assets.Models.Asset>> GetListByAccountIdAsync(
		AccountId accountId,
		PageRequest pageRequest,
		CancellationToken ct);

	public Task<PagedResult<Core.Assets.Models.Asset>> GetListByAccountIdAndPlatformIdAsync(
		AccountId accountId,
		PlatformId platformId,
		PageRequest pageRequest,
		CancellationToken ct);
}
