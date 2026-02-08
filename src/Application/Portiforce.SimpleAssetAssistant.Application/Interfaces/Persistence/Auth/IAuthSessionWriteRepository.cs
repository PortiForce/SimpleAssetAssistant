using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;

public interface IAuthSessionWriteRepository
{
	Task AddAsync(AuthSessionToken token, CancellationToken ct);

	/// <summary>
	/// Updates an existing token (mostly for Revocation).
	/// </summary>
	Task UpdateAsync(AuthSessionToken token, CancellationToken ct);

	/// <summary>
	/// Bulk update for security events (e.g. revoking all tokens for a compromised user).
	/// </summary>
	Task RevokeAllForUserAsync(AccountId accountId, CancellationToken ct);
}
