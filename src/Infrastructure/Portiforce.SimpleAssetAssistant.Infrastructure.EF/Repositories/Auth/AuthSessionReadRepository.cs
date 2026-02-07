using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Auth;

internal sealed class AuthSessionReadRepository(AssetAssistantDbContext db) : IAuthSessionReadRepository
{
	public Task<AuthSessionToken?> GetByHashAsync(string tokenHash, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<List<AuthSessionToken>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
