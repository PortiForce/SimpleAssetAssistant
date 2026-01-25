using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Asset;

public interface IAssetLookupService
{
	Task<IReadOnlyDictionary<AssetId, AssetListItem>> GetRequiredAsync(
		IReadOnlyCollection<AssetId> ids,
		CancellationToken ct);
}
