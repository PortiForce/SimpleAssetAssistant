using Portiforce.SAA.Application.Interfaces.Persistence.PlatformAccount;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.PlatformAccount;

internal sealed class PlatformAccountWriteRepository(AssetAssistantDbContext db) : IPlatformAccountWriteRepository
{
	public Task AddAsync(Core.Assets.Models.PlatformAccount entity, CancellationToken ct)
	{
		return db.PlatformAccounts.AddAsync(entity, ct).AsTask();
	}

	public Task UpdateAsync(Core.Assets.Models.PlatformAccount platformAccount, CancellationToken ct)
	{
		db.PlatformAccounts.Update(platformAccount);
		return Task.CompletedTask;
	}
}
