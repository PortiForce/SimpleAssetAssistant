using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Auth;

internal sealed class AuthSessionWriteRepository(AssetAssistantDbContext db) : IAuthSessionWriteRepository
{
	public Task AddAsync(AuthSessionToken token, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task UpdateAsync(AuthSessionToken token, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task RevokeAllForUserAsync(AccountId accountId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
