using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;

public sealed class AuthSessionToken : Entity<Guid>
{
	private AuthSessionToken(
		TenantId tenantId,
		AccountId accountId,
		Guid sessionId,
		string tokenHash,
		DateTimeOffset createdAt,
		string createdByIp,
		string createdUserAgent,
		DateTimeOffset expiresAt)
	{
		if (sessionId == Guid.Empty)
		{
			throw new DomainValidationException("SessionId is not defined");
		}

		if (string.IsNullOrEmpty(tokenHash)) 
		{
			throw new DomainValidationException("tokenHash must be defined.");
		}

		if (accountId.IsEmpty)
		{
			throw new DomainValidationException("AccountId must be defined.");
		}

		if (tenantId.IsEmpty)
		{
			throw new DomainValidationException("TenantId must be defined.");
		}

		if (expiresAt < createdAt)
		{
			throw new DomainValidationException("Invalid token live time");
		}

		Id = Guid.NewGuid();
		TenantId = tenantId;
		AccountId = accountId;
		SessionId = sessionId;
		TokenHash = tokenHash;
		CreatedByIp = createdByIp;
		CreatedUserAgent = createdUserAgent;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}

	// Private Empty Constructor for EF Core
	private AuthSessionToken()
	{

	}

	public TenantId TenantId { get; init; }
	public AccountId AccountId { get; init; }
	public Guid SessionId { get; init; }
	public string TokenHash { get; private set; }

	public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
	public DateTimeOffset ExpiresAt { get; init; }

	// Audit
	public string CreatedByIp { get; init; }
	public string CreatedUserAgent { get; init; }

	// Revocation / Rotation
	public DateTimeOffset? RevokedAt { get; private set; }
	public string? RevokedByIp { get; private set; }
	public string? ReplacedByTokenHash { get; private set; }
	public TokenRevokeReason? RevokedReason { get; private set; }

	public static AuthSessionToken Create(
		TenantId tenantId,
		AccountId accountId,
		Guid sessionId,
		string tokenHash,
		string createdByIp,
		string createdUserAgent,
		DateTimeOffset nowUtc,
		DateTimeOffset expiresAt)
	{
		return new AuthSessionToken(
			tenantId,
			accountId,
			sessionId,
			tokenHash,
			nowUtc,
			createdByIp,
			createdUserAgent,
			expiresAt);
	}

	public bool IsRevoked => RevokedAt is not null;

	public bool IsActive(DateTimeOffset nowUtc) => !IsRevoked && ExpiresAt > nowUtc;

	public void Revoke(
		DateTimeOffset nowUtc,
		TokenRevokeReason reason,
		string ip,
		string? replacedByHash = null)
	{
		// idempotent flow check
		if (IsRevoked)
		{
			return;
		}

		RevokedAt = nowUtc;
		RevokedReason = reason;
		RevokedByIp = ip;
		ReplacedByTokenHash = replacedByHash;
	}
}
