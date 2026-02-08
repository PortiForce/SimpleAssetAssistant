using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;

internal sealed class ExternalIdentityWriteRepository(AssetAssistantDbContext db) : IExternalIdentityWriteRepository
{
	public async Task AddAsync(ExternalIdentity externalIdentity, CancellationToken ct)
	{
		await db.ExternalIdentities.AddAsync(externalIdentity, ct);
	}
}
