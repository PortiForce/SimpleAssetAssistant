using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Auth;

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
