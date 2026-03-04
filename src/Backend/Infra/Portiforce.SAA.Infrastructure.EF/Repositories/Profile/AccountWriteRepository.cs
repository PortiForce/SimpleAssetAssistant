using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Profile;

internal sealed class AccountWriteRepository(AssetAssistantDbContext db) : IAccountWriteRepository
{
	public Task AddAsync(Account entity, CancellationToken ct)
	{
		return db.Accounts.AddAsync(entity, ct).AsTask();
	}

	public Task UpdateAsync(Account entity, CancellationToken ct)
	{
		db.Accounts.Update(entity);
		return Task.CompletedTask;
	}
}
