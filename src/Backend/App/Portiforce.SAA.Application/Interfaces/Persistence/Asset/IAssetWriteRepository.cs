using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Asset;

public interface IAssetWriteRepository : IWriteRepository<Core.Assets.Models.Asset, AssetId>
{
	Task UpdateAsync(Core.Assets.Models.Asset asset, CancellationToken ct);
}
