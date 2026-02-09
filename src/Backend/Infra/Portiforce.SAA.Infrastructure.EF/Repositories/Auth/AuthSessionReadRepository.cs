using Microsoft.EntityFrameworkCore;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Auth;

internal sealed class AuthSessionReadRepository(AssetAssistantDbContext db) : IAuthSessionReadRepository
{
	public Task<AuthSessionToken?> GetByHashAsync(byte[] tokenHash, CancellationToken ct)
	{
		return db.AuthSessionTokens
			.AsNoTracking()
			.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
	}

	public Task<List<AuthSessionToken>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
