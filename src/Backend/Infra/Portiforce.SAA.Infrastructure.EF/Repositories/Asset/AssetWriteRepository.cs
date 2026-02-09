using Portiforce.SAA.Application.Interfaces.Persistence.Asset;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Asset;

internal sealed class AssetWriteRepository : IAssetWriteRepository
{
	public Task AddAsync(Core.Assets.Models.Asset entity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task UpdateAsync(Core.Assets.Models.Asset asset, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
