using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Identity.Models.Auth;

public sealed class AuthSessionTokenTests
{
	[Fact]
	public void Create_ShouldSetProperties()
	{
		TenantId tenantId = TenantId.New();
		AccountId accountId = AccountId.New();
		Guid sessionId = Guid.NewGuid();
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;
		DateTimeOffset expiresAt = now.AddHours(1);

		AuthSessionToken token = AuthSessionToken.Create(
			tenantId,
			accountId,
			sessionId,
			tokenHash,
			"127.0.0.1",
			"Mozilla",
			now,
			expiresAt);

		_ = token.Id.Should().NotBe(Guid.Empty);
		_ = token.TenantId.Should().Be(tenantId);
		_ = token.AccountId.Should().Be(accountId);
		_ = token.SessionId.Should().Be(sessionId);
		_ = token.TokenHash.Should().Equal(tokenHash);
		_ = token.CreatedByIp.Should().Be("127.0.0.1");
		_ = token.CreatedUserAgent.Should().Be("Mozilla");
		_ = token.CreatedAt.Should().Be(now);
		_ = token.ExpiresAt.Should().Be(expiresAt);
		_ = token.IsRevoked.Should().BeFalse();
	}

	[Fact]
	public void Create_WhenSessionIdIsEmpty_ShouldThrow()
	{
		Func<AuthSessionToken> act = () => AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.Empty,
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			DateTimeOffset.UtcNow,
			DateTimeOffset.UtcNow.AddHours(1));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*SessionId is not defined*");
	}

	[Fact]
	public void Create_WhenTokenHashIsNull_ShouldThrow()
	{
		Func<AuthSessionToken> act = () => AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			null!,
			"127.0.0.1",
			"Mozilla",
			DateTimeOffset.UtcNow,
			DateTimeOffset.UtcNow.AddHours(1));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*tokenHash must be defined as 32 bytes*");
	}

	[Fact]
	public void Create_WhenTokenHashLengthIsInvalid_ShouldThrow()
	{
		Func<AuthSessionToken> act = () => AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			new byte[31],
			"127.0.0.1",
			"Mozilla",
			DateTimeOffset.UtcNow,
			DateTimeOffset.UtcNow.AddHours(1));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*tokenHash must be defined as 32 bytes*");
	}

	[Fact]
	public void Create_WhenAccountIdIsEmpty_ShouldThrow()
	{
		Func<AuthSessionToken> act = () => AuthSessionToken.Create(
			TenantId.New(),
			AccountId.Empty,
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			DateTimeOffset.UtcNow,
			DateTimeOffset.UtcNow.AddHours(1));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*AccountId must be defined*");
	}

	[Fact]
	public void Create_WhenTenantIdIsEmpty_ShouldThrow()
	{
		Func<AuthSessionToken> act = () => AuthSessionToken.Create(
			TenantId.Empty,
			AccountId.New(),
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			DateTimeOffset.UtcNow,
			DateTimeOffset.UtcNow.AddHours(1));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*TenantId must be defined*");
	}

	[Fact]
	public void Create_WhenExpiresBeforeCreated_ShouldThrow()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;

		Func<AuthSessionToken> act = () => AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			now,
			now.AddMinutes(-1));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*Invalid token live time*");
	}

	[Fact]
	public void IsActive_WhenNotRevokedAndNotExpired_ShouldReturnTrue()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;

		AuthSessionToken token = AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			now,
			now.AddHours(1));

		_ = token.IsActive(now.AddMinutes(1)).Should().BeTrue();
	}

	[Fact]
	public void IsActive_WhenExpired_ShouldReturnFalse()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;

		AuthSessionToken token = AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			now,
			now.AddMinutes(1));

		_ = token.IsActive(now.AddHours(1)).Should().BeFalse();
	}

	[Fact]
	public void Revoke_ShouldSetRevocationFields()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;

		AuthSessionToken token = AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			now,
			now.AddHours(1));

		byte[] replacement = CreateTokenHash();
		DateTimeOffset revokedAt = now.AddMinutes(10);

		token.Revoke(
			revokedAt,
			TokenRevokeReason.ReplacedByNewToken,
			"10.10.10.10",
			replacement);

		_ = token.IsRevoked.Should().BeTrue();
		_ = token.RevokedAt.Should().Be(revokedAt);
		_ = token.RevokedReason.Should().Be(TokenRevokeReason.ReplacedByNewToken);
		_ = token.RevokedByIp.Should().Be("10.10.10.10");
		_ = token.ReplacedByTokenHash.Should().Equal(replacement);
	}

	[Fact]
	public void Revoke_WhenAlreadyRevoked_ShouldBeIdempotent()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;

		AuthSessionToken token = AuthSessionToken.Create(
			TenantId.New(),
			AccountId.New(),
			Guid.NewGuid(),
			CreateTokenHash(),
			"127.0.0.1",
			"Mozilla",
			now,
			now.AddHours(1));

		DateTimeOffset firstRevokeAt = now.AddMinutes(5);
		token.Revoke(firstRevokeAt, TokenRevokeReason.UserLogout, "1.1.1.1");

		token.Revoke(now.AddMinutes(10), TokenRevokeReason.TheftDetected, "2.2.2.2", CreateTokenHash());

		_ = token.RevokedAt.Should().Be(firstRevokeAt);
		_ = token.RevokedReason.Should().Be(TokenRevokeReason.UserLogout);
		_ = token.RevokedByIp.Should().Be("1.1.1.1");
	}

	private static byte[] CreateTokenHash() => Enumerable.Range(1, 32).Select(static i => (byte) i).ToArray();
}