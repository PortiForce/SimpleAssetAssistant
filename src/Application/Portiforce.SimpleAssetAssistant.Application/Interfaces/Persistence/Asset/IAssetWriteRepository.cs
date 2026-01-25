using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;

public interface IAssetWriteRepository : IWriteRepository<Portiforce.SimpleAssetAssistant.Core.Assets.Models.Asset, AssetId>
{
	Task UpdateAsync(Core.Assets.Models.Asset asset, CancellationToken ct);
}
