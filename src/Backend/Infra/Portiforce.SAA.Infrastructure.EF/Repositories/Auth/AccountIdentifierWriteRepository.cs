using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Auth;

internal sealed class AccountIdentifierWriteRepository(AssetAssistantDbContext db) : IAccountIdentifierWriteRepository
{
	public async Task AddAsync(AccountIdentifier accountIdentifier, CancellationToken ct) =>
		_ = await db.AccountIdentifiers.AddAsync(accountIdentifier, ct);
}