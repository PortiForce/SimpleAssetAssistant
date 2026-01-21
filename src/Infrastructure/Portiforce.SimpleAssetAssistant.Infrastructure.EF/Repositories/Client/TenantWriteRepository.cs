using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Client;

internal sealed class TenantWriteRepository(AssetAssistantDbContext db) : ITenantWriteRepository
{
	public Task AddAsync(Tenant entity, CancellationToken ct)
	{
		return db.Tenants.AddAsync(entity, ct).AsTask();
	}
		

	public Task UpdateAsync(Tenant tenant, CancellationToken ct)
	{
		db.Tenants.Update(tenant);
		return Task.CompletedTask;
	}
}
