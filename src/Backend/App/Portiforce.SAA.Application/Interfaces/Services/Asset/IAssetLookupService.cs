using Portiforce.SAA.Application.UseCases.Asset.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Services.Asset;

public interface IAssetLookupService
{
	Task<IReadOnlyDictionary<AssetId, AssetListItem>> GetRequiredAsync(
		IReadOnlyCollection<AssetId> ids,
		CancellationToken ct);
}
