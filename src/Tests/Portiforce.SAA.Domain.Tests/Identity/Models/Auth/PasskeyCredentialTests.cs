using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Identity.Models.Auth;

public sealed class PasskeyCredentialTests
{
	[Fact]
	public void Register_ShouldSetProperties()
	{
		AccountId accountId = AccountId.New();
		byte[] publicKey = [1, 2, 3, 4];

		PasskeyCredential credential = PasskeyCredential.Register(
			accountId,
			"cred-123",
			publicKey,
			7,
			"user-handle");

		_ = credential.Id.IsEmpty.Should().BeFalse();
		_ = credential.AccountId.Should().Be(accountId);
		_ = credential.CredentialId.Should().Be("cred-123");
		_ = credential.PublicKey.Should().Equal(publicKey);
		_ = credential.SignatureCounter.Should().Be(7U);
		_ = credential.UserHandle.Should().Be("user-handle");
		_ = credential.LastUsedAt.Should().BeNull();
		_ = credential.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void Register_WhenUserHandleIsNull_ShouldAllowNull()
	{
		PasskeyCredential credential = PasskeyCredential.Register(
			AccountId.New(),
			"cred-123",
			[1, 2, 3],
			0,
			null);

		_ = credential.UserHandle.Should().BeNull();
	}

	[Fact]
	public void UpdateCounter_ShouldSetCounter_AndLastUsedAt()
	{
		PasskeyCredential credential = PasskeyCredential.Register(
			AccountId.New(),
			"cred-123",
			[1, 2, 3],
			1,
			"user-handle");

		credential.UpdateCounter(99);

		_ = credential.SignatureCounter.Should().Be(99U);
		_ = credential.LastUsedAt.Should().NotBeNull();
		_ = credential.LastUsedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void UpdateCounter_WhenCalledTwice_ShouldOverwriteCounter_AndRefreshTimestamp()
	{
		PasskeyCredential credential = PasskeyCredential.Register(
			AccountId.New(),
			"cred-123",
			[1, 2, 3],
			1,
			"user-handle");

		credential.UpdateCounter(5);
		DateTimeOffset? firstLastUsedAt = credential.LastUsedAt;

		Thread.Sleep(10);

		credential.UpdateCounter(6);

		_ = credential.SignatureCounter.Should().Be(6U);
		_ = credential.LastUsedAt.Should().NotBeNull();
		_ = credential.LastUsedAt.Should().BeAfter(firstLastUsedAt!.Value);
	}
}