using Portiforce.SAA.Application.Interfaces.Persistence.Asset;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Asset;

internal sealed class AssetWriteRepository(AssetAssistantDbContext db) : IAssetWriteRepository
{
	public Task AddAsync(Core.Assets.Models.Asset entity, CancellationToken ct) =>
		db.Assets.AddAsync(entity, ct).AsTask();

	public Task UpdateAsync(Core.Assets.Models.Asset asset, CancellationToken ct)
	{
		_ = db.Assets.Update(asset);
		return Task.CompletedTask;
	}
}