using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Auth;

internal sealed class ExternalIdentityWriteRepository(AssetAssistantDbContext db) : IExternalIdentityWriteRepository
{
	public async Task AddAsync(ExternalIdentity externalIdentity, CancellationToken ct)
	{
		await db.ExternalIdentities.AddAsync(externalIdentity, ct);
	}
}
