using System.Security;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Security;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Time;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Auth;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Identity.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Flow.Services;

internal sealed class AuthService(
	ITokenGenerator tokenGenerator,
	IHashingService hashingService,
	IClock clock,
	IAuthSessionReadRepository sessionReadRepository,
	IAuthSessionWriteRepository sessionWriteRepository,
	IAccountReadRepository accountReadRepository,
	IUnitOfWork unitOfWork) : IAuthService
{
	public async Task<AuthResponse> RefreshTokenAsync(
		string rawRefreshToken,
		string? ipAddress,
		string? userAgent,
		CancellationToken ct)
	{
		var now = clock.UtcNow;

		byte[] incomingHash = hashingService.HashRefreshToken(rawRefreshToken);

		AuthSessionToken? existing = await sessionReadRepository.GetByHashAsync(incomingHash, ct);
		if (existing is null)
		{
			throw new InvalidRefreshTokenException("Invalid refresh token.");
		}

		// replay detection
		if (existing.IsRevoked)
		{
			await RevokeSessionChainAsync(existing.SessionId, TokenRevokeReason.TheftDetected, ipAddress, ct);
			throw new SecurityException("Token reuse detected.");
		}

		if (!existing.IsActive(now))
		{
			throw new InvalidRefreshTokenException("Refresh token expired.");
		}

		var tenantId = existing.TenantId;
		var accountId = existing.AccountId;

		// ensure account still active
		var account = await accountReadRepository.GetForAuthAsync(tenantId, accountId, ct);
		if (account is null || account.State != AccountState.Active)
		{
			throw new SecurityException("Account is not active.");
		}

		var newAccess = tokenGenerator.GenerateAccessToken(account);

		var newRawRefresh = tokenGenerator.GenerateRefreshToken();
		var newTokenHash = hashingService.HashRefreshToken(newRawRefresh);

		var expiresAt = now.AddHours( /* from settings */ 168);

		var newEntity = AuthSessionToken.Create(
			tenantId,
			accountId,
			existing.SessionId,
			newTokenHash,
			ipAddress,
			userAgent,
			now,
			expiresAt);

		existing.Revoke(
			nowUtc: now,
			reason: TokenRevokeReason.ReplacedByNewToken,
			ip: ipAddress,
			replacedByHash: newTokenHash);

		await sessionWriteRepository.AddAsync(newEntity, ct);
		await sessionWriteRepository.UpdateAsync(existing, ct);
		await unitOfWork.SaveChangesAsync(ct);

		return new AuthResponse(newAccess, newRawRefresh, expiresAt.UtcDateTime);
	}

	private async Task RevokeSessionChainAsync(Guid sessionId, TokenRevokeReason reason, string? ip, CancellationToken ct)
	{
		var now = clock.UtcNow;
		var tokens = await sessionReadRepository.GetBySessionIdAsync(sessionId, ct);

		foreach (AuthSessionToken t in tokens.Where(t => t.IsActive(now)))
		{
			t.Revoke(now, reason, ip);
			await sessionWriteRepository.UpdateAsync(t, ct);
		}

		await unitOfWork.SaveChangesAsync(ct);
	}

	public async Task RevokeRefreshTokenAsync(string rawRefreshToken, string? ip, CancellationToken ct)
	{
		var now = clock.UtcNow;
		var tokenHash = hashingService.HashRefreshToken(rawRefreshToken);

		var existingToken = await sessionReadRepository.GetByHashAsync(tokenHash, ct);
		if (existingToken is null)
		{
			// idempotent logout
			return; 
		}

		if (existingToken.IsActive(now))
		{
			existingToken.Revoke(now, TokenRevokeReason.UserLogout, ip);
			await sessionWriteRepository.UpdateAsync(existingToken, ct);
			await unitOfWork.SaveChangesAsync(ct);
		}
	}
}
