using Portiforce.SAA.Core.Identity.Models.Auth;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IAuthSessionReadRepository
{
	/// <summary>
	/// Finds a token by its secure hash.
	/// Used during Refresh Token flow to validate the incoming token.
	/// </summary>
	Task<AuthSessionToken?> GetByHashAsync(byte[] tokenHash, CancellationToken ct);

	/// <summary>
	/// Finds the entire chain of tokens for a specific session.
	/// Used for "Theft Detection" (revoking the whole family).
	/// </summary>
	Task<List<AuthSessionToken>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct);
}
