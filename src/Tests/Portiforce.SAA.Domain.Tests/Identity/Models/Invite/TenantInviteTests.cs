using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Identity.Models.Invite;

public sealed class TenantInviteTests
{
	[Fact]
	public void Create_WhenTenantIdIsEmpty_ShouldThrow()
	{
		InviteTarget inviteTarget = InviteTarget.Email("user@example.com");
		AccountId invitedByAccountId = AccountId.New();
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.Empty,
			inviteTarget,
			invitedByAccountId,
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("tenantId");
	}

	[Fact]
	public void Create_WhenInvitedByAccountIdIsEmpty_ShouldThrow()
	{
		InviteTarget inviteTarget = InviteTarget.Email("user@example.com");
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.Empty,
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("invitedByAccountId");
	}

	[Fact]
	public void Create_WhenExpiresAtIsNotInFuture_ShouldThrow()
	{
		InviteTarget inviteTarget = InviteTarget.Email("user@example.com");
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("expiresAtUtc");
	}

	[Fact]
	public void Create_WhenInviteTargetIsEmpty_ShouldThrow()
	{
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		InviteTarget inviteTarget = InviteTarget.Restore(
				"dummy@example.com",
				InviteChannel.Email,
				InviteTargetKind.Email) with
		{
			Value = "   ",
			Kind = InviteTargetKind.None
		};

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("inviteTarget");
	}

	[Fact]
	public void Create_WhenAliasIsNull_ShouldThrow()
	{
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		InviteTarget inviteTarget = InviteTarget.Restore(
				"dummy@example.com",
				InviteChannel.Email,
				InviteTargetKind.Email) with
		{
			Value = "   ",
			Kind = InviteTargetKind.None
		};

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			null,
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("inviteTarget");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("a")]
	[InlineData("al")]
	[InlineData("al   ")]
	[InlineData("   al   ")]
	public void Create_WhenAliasIsToShort_ShouldThrow(string alias)
	{
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		InviteTarget inviteTarget = InviteTarget.Restore(
				"dummy@example.com",
				InviteChannel.Email,
				InviteTargetKind.Email) with
		{
			Value = "   ",
			Kind = InviteTargetKind.None
		};

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			alias,
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("inviteTarget");
	}

	[Fact]
	public void Create_WhenAliasIsToLarge_ShouldThrow()
	{
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		InviteTarget inviteTarget = InviteTarget.Restore(
				"dummy@example.com",
				InviteChannel.Email,
				InviteTargetKind.Email) with
		{
			Value = "   ",
			Kind = InviteTargetKind.None
		};

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			new string('A', 101),
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("inviteTarget");
	}

	[Fact]
	public void Create_WhenTokenHashIsNull_ShouldThrow()
	{
		InviteTarget inviteTarget = InviteTarget.Email("user@example.com");
		DateTimeOffset now = DateTimeOffset.UtcNow;

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			null!,
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*tokenHash must be defined as 32 bytes*");
	}

	[Fact]
	public void Create_WhenTokenHashLengthIsNot32_ShouldThrow()
	{
		InviteTarget inviteTarget = InviteTarget.Email("user@example.com");
		DateTimeOffset now = DateTimeOffset.UtcNow;

		Func<TenantInvite> act = () => TenantInvite.Create(
			TenantId.New(),
			inviteTarget,
			AccountId.New(),
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			new byte[31],
			now,
			now.AddDays(7));

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*tokenHash must be defined as 32 bytes*");
	}

	[Fact]
	public void Create_WhenValid_ShouldSucceed()
	{
		TenantId tenantId = TenantId.New();
		InviteTarget inviteTarget = InviteTarget.Email("user@example.com");
		AccountId invitedByAccountId = AccountId.New();
		byte[] tokenHash = CreateTokenHash();
		DateTimeOffset now = DateTimeOffset.UtcNow;
		DateTimeOffset expiresAtUtc = now.AddDays(7);

		TenantInvite invite = TenantInvite.Create(
			tenantId,
			inviteTarget,
			invitedByAccountId,
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			tokenHash,
			now,
			expiresAtUtc);

		_ = invite.Id.Should().NotBe(Guid.Empty);
		_ = invite.TenantId.Should().Be(tenantId);
		_ = invite.InviteTarget.Should().Be(inviteTarget);
		_ = invite.InvitedByAccountId.Should().Be(invitedByAccountId);
		_ = invite.IntendedRole.Should().Be(Role.TenantUser);
		_ = invite.IntendedTier.Should().Be(AccountTier.Observer);
		_ = invite.State.Should().Be(InviteState.Created);
		_ = invite.TokenHash.Should().Equal(tokenHash);
		_ = invite.CreatedAtUtc.Should().Be(now);
		_ = invite.ExpiresAtUtc.Should().Be(expiresAtUtc);
		_ = invite.SentAtUtc.Should().BeNull();
		_ = invite.SendCount.Should().Be(0);
		_ = invite.UpdatedAtUtc.Should().BeNull();
		_ = invite.AcceptedAccountId.Should().BeNull();
		_ = invite.RevokedByAccountId.Should().BeNull();
	}

	[Fact]
	public void Create_WhenInviteIdProvided_ShouldUseProvidedId()
	{
		Guid inviteId = Guid.NewGuid();
		DateTimeOffset now = DateTimeOffset.UtcNow;

		TenantInvite invite = TenantInvite.Create(
			TenantId.New(),
			InviteTarget.Email("user@example.com"),
			AccountId.New(),
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			CreateTokenHash(),
			now,
			now.AddDays(7),
			inviteId);

		_ = invite.Id.Should().Be(inviteId);
	}

	[Fact]
	public void MarkSent_WhenStateIsCreated_ShouldSetSentStateAndIncrementCount()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		invite.MarkSent(now.AddMinutes(1));

		_ = invite.State.Should().Be(InviteState.Sent);
		_ = invite.SentAtUtc.Should().Be(now.AddMinutes(1));
		_ = invite.SendCount.Should().Be(1);
	}

	[Fact]
	public void MarkSent_WhenCalledTwice_ShouldUpdateSentAtAndIncrementCount()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		invite.MarkSent(now.AddMinutes(1));
		invite.MarkSent(now.AddMinutes(2));

		_ = invite.State.Should().Be(InviteState.Sent);
		_ = invite.SentAtUtc.Should().Be(now.AddMinutes(2));
		_ = invite.SendCount.Should().Be(2);
	}

	[Fact]
	public void MarkSent_WhenInviteAccepted_ShouldThrow()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Accept(AccountId.New(), now.AddMinutes(1));

		Action act = () => invite.MarkSent(now.AddMinutes(2));

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Cannot send invite in status 'Accepted'*");
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void MarkSent_WhenInviteRevoked_ShouldThrow(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Revoke(AccountId.New(), now.AddMinutes(1), blockFutureInvites);

		Action act = () => invite.MarkSent(now.AddMinutes(2));

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Cannot send invite in status 'RevokedByTenant'*");
	}

	[Fact]
	public void MarkSent_WhenInviteExpired_ShouldThrow()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddMinutes(1));

		Action act = () => invite.MarkSent(now.AddMinutes(1));

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Cannot send an expired invite.*");
	}

	[Fact]
	public void Accept_WhenAccountIdIsEmpty_ShouldThrow()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		Action act = () => invite.Accept(AccountId.Empty, now.AddMinutes(1));

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("accountId");
	}

	[Fact]
	public void Accept_WhenValid_ShouldSetAcceptedStateAndAccount()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		AccountId accountId = AccountId.New();
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		_ = invite.Accept(accountId, now.AddMinutes(1));

		_ = invite.State.Should().Be(InviteState.Accepted);
		_ = invite.AcceptedAccountId.Should().Be(accountId);
		_ = invite.UpdatedAtUtc.Should().Be(now.AddMinutes(1));
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Accept_WhenInviteRevoked_ShouldThrow(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Revoke(AccountId.New(), now.AddMinutes(1), blockFutureInvites);

		Action act = () => invite.Accept(AccountId.New(), now.AddMinutes(2));

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Invite revoked.*");
	}

	[Fact]
	public void Accept_WhenInviteExpired_ShouldThrow()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddMinutes(1));

		Action act = () => invite.Accept(AccountId.New(), now.AddMinutes(1));

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Invite expired.*");
	}

	[Fact]
	public void Accept_WhenInviteAlreadyAccepted_ShouldReturnFalse()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		AccountId accountId = AccountId.New();
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		bool firstResult = invite.Accept(accountId, now.AddMinutes(1));
		bool secondResult = invite.Accept(accountId, now.AddMinutes(2));

		_ = firstResult.Should().BeTrue();
		_ = secondResult.Should().BeFalse();
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Revoke_WhenRevokedByAccountIdIsEmpty_ShouldThrow(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		Action act = () => invite.Revoke(AccountId.Empty, now.AddMinutes(1), blockFutureInvites);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("revokedByAccountId");
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Revoke_WhenValid_ShouldSetRevokedStateAndActor(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		AccountId revokedByAccountId = AccountId.New();
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		_ = invite.Revoke(revokedByAccountId, now.AddMinutes(1), blockFutureInvites);

		_ = invite.State.Should().Be(InviteState.RevokedByTenant);
		_ = invite.RevokedByAccountId.Should().Be(revokedByAccountId);
		_ = invite.UpdatedAtUtc.Should().Be(now.AddMinutes(1));
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Revoke_WhenInviteAccepted_ShouldThrow(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Accept(AccountId.New(), now.AddMinutes(1));

		Action act = () => invite.Revoke(AccountId.New(), now.AddMinutes(2), blockFutureInvites);

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Cannot revoke an accepted invite.*");
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Revoke_WhenInviteDeclined_ShouldThrow(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Decline(now.AddMinutes(1));

		Action act = () => invite.Revoke(AccountId.New(), now.AddMinutes(2), blockFutureInvites);

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Cannot revoke declined invite.*");
	}

	[Fact]
	public void Decline_WhenValid_ShouldSetDeclinedState()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		_ = invite.Decline(now.AddMinutes(1));

		_ = invite.State.Should().Be(InviteState.DeclinedByUser);
		_ = invite.UpdatedAtUtc.Should().Be(now.AddMinutes(1));
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Decline_WhenInviteRevoked_ShouldStillSucceed(bool blockFutureInvites)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Revoke(AccountId.New(), now.AddMinutes(1), blockFutureInvites);

		_ = invite.Decline(now.AddMinutes(2));

		_ = invite.State.Should().Be(InviteState.RevokedByTenant);
		_ = invite.BlockFutureInvites.Should().BeTrue();
		_ = invite.UpdatedAtUtc.Should().Be(now.AddMinutes(2));
	}

	[Fact]
	public void Decline_WhenInviteAccepted_ShouldThrow()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));
		_ = invite.Accept(AccountId.New(), now.AddMinutes(1));

		Action act = () => invite.Decline(now.AddMinutes(2));

		_ = act.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("*Cannot decline an accepted invite.*");
	}

	[Fact]
	public void IsExpired_WhenNowBeforeExpiresAtAndNotAccepted_ShouldReturnFalse()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		TenantInvite invite = CreateInvite(now, now.AddDays(7));

		bool result = invite.IsExpired(now.AddDays(1));

		_ = result.Should().BeFalse();
	}

	[Fact]
	public void IsExpired_WhenNowEqualsExpiresAtAndNotAccepted_ShouldReturnTrue()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		DateTimeOffset expiresAt = now.AddDays(7);
		TenantInvite invite = CreateInvite(now, expiresAt);

		bool result = invite.IsExpired(expiresAt);

		_ = result.Should().BeTrue();
	}

	[Fact]
	public void IsExpired_WhenNowAfterExpiresAtAndNotAccepted_ShouldReturnTrue()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		DateTimeOffset expiresAt = now.AddDays(7);
		TenantInvite invite = CreateInvite(now, expiresAt);

		bool result = invite.IsExpired(expiresAt.AddMinutes(1));

		_ = result.Should().BeTrue();
	}

	[Fact]
	public void IsExpired_WhenInviteAccepted_ShouldReturnFalseEvenAfterExpiration()
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		DateTimeOffset expiresAt = now.AddDays(7);
		TenantInvite invite = CreateInvite(now, expiresAt);
		_ = invite.Accept(AccountId.New(), now.AddMinutes(1));

		bool result = invite.IsExpired(expiresAt.AddDays(1));

		_ = result.Should().BeFalse();
	}

	private static TenantInvite CreateInvite(DateTimeOffset now, DateTimeOffset expiresAtUtc)
	{
		return TenantInvite.Create(
			TenantId.New(),
			InviteTarget.Email("user@example.com"),
			AccountId.New(),
			"alias",
			Role.TenantUser,
			AccountTier.Observer,
			CreateTokenHash(),
			now,
			expiresAtUtc);
	}

	private static byte[] CreateTokenHash() => new byte[32];
}