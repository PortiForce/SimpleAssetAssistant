using Portiforce.SAA.Application.Interfaces.Persistence.Platform;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Platform;

internal sealed class PlatformWriteRepository(AssetAssistantDbContext db) : IPlatformWriteRepository
{
	public Task AddAsync(Core.Assets.Models.Platform entity, CancellationToken ct) =>
		db.Platforms.AddAsync(entity, ct).AsTask();

	public Task UpdateAsync(Core.Assets.Models.Platform platform, CancellationToken ct)
	{
		_ = db.Platforms.Update(platform);
		return Task.CompletedTask;
	}
}