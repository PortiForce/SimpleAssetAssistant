using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Persistence.Asset;
using Portiforce.SAA.Application.Interfaces.Services.Asset;
using Portiforce.SAA.Application.UseCases.Asset.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Flow.Services;

internal sealed class AssetLookupService(IAssetReadRepository assetReadRepository) : IAssetLookupService
{
	public async Task<IReadOnlyDictionary<AssetId, AssetListItem>> GetRequiredAsync(
		IReadOnlyCollection<AssetId> ids,
		CancellationToken ct)
	{
		var assetList = await assetReadRepository.GetListByAssetIdsAsync(ids, ct);

		Dictionary<AssetId, AssetListItem> fetchedAssetsMap = assetList.ToDictionary(a => a.Id);

		foreach (var assetId in ids)
		{
			if (!fetchedAssetsMap.TryGetValue(assetId, out var asset))
			{
				throw new NotFoundException("Asset", assetId);
			}
		}

		return fetchedAssetsMap;
	}
}
