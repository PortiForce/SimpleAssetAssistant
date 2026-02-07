using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Extensions;
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
		byte[] tokenHash,
		DateTimeOffset createdAt,
		string createdByIp,
		string createdUserAgent,
		DateTimeOffset expiresAt)
	{
		if (sessionId == Guid.Empty)
		{
			throw new DomainValidationException("SessionId is not defined");
		}

		if (tokenHash is null || tokenHash.Length != 32) 
		{
			throw new DomainValidationException("tokenHash must be defined as 32 bytes");
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

		Id = GuidExtensions.New();
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
	public byte[] TokenHash { get; private set; }

	public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
	public DateTimeOffset ExpiresAt { get; init; }

	// Audit
	public string? CreatedByIp { get; init; }
	public string? CreatedUserAgent { get; private set; }
	public string? UserAgentFingerprint { get; private set; }

	// Revocation / Rotation
	public DateTimeOffset? RevokedAt { get; private set; }
	public string? RevokedByIp { get; private set; }
	public byte[]? ReplacedByTokenHash { get; private set; }
	public TokenRevokeReason? RevokedReason { get; private set; }

	public static AuthSessionToken Create(
		TenantId tenantId,
		AccountId accountId,
		Guid sessionId,
		byte[] tokenHash,
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
		byte[]? replacedByHash = null)
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
