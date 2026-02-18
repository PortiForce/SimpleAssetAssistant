using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Identity.Models.Invite;

public sealed class TenantInvite : Entity<Guid>, IAggregateRoot
{
	// Tenant scoping
	public TenantId TenantId { get; private set; }

	// Target user info
	public InviteTarget InviteTarget { get; private set; } = default!;

	// Who invited + intended permission set
	public AccountId InvitedByAccountId { get; private set; }
	public InviteTenantRole IntendedRole { get; private set; }
	public InviteAccountTier IntendedTier { get; private set; }

	public byte[] TokenHash { get; private set; } = default!;

	// Lifecycle
	public InviteState State { get; private set; } = InviteState.Created;
	public DateTimeOffset CreatedAtUtc { get; private set; }
	public DateTimeOffset ExpiresAtUtc { get; private set; }

	public DateTimeOffset? SentAtUtc { get; private set; }
	public int SendCount { get; private set; }

	public DateTimeOffset? AcceptedAtUtc { get; private set; }
	public AccountId? AcceptedAccountId { get; private set; }

	public DateTimeOffset? RevokedAtUtc { get; private set; }
	public AccountId? RevokedByAccountId { get; private set; }

	// EF
	private TenantInvite() { }

	public static TenantInvite Create(
		TenantId tenantId,
		InviteTarget inviteTarget,
		AccountId invitedByAccountId,
		InviteTenantRole intendedRole,
		InviteAccountTier intendedTier,
		byte[] tokenHash,
		DateTimeOffset now,
		DateTimeOffset expiresAtUtc)
	{
		if (tenantId == TenantId.Empty)
		{
			throw new ArgumentException("TenantId is required.", nameof(tenantId));
		}

		if (invitedByAccountId == AccountId.Empty)
		{
			throw new ArgumentException("InvitedByAccountId is required.", nameof(invitedByAccountId));
		}
		if (expiresAtUtc <= now)
		{
			throw new ArgumentException("ExpiresAt must be in the future.", nameof(expiresAtUtc));
		}
		if (inviteTarget is null)
		{
			throw new ArgumentNullException(nameof(inviteTarget));
		}
		if (tokenHash is null || tokenHash.Length != 32)
		{
			throw new DomainValidationException("tokenHash must be defined as 32 bytes");
		}

		return new TenantInvite
		{
			TenantId = tenantId,
			InviteTarget = inviteTarget,
			InvitedByAccountId = invitedByAccountId,
			IntendedRole = intendedRole,
			IntendedTier = intendedTier,
			State = InviteState.Created,
			TokenHash = tokenHash,
			CreatedAtUtc = now,
			ExpiresAtUtc = expiresAtUtc
		};
	}

	public void MarkSent(DateTimeOffset nowUtc)
	{
		if (State is InviteState.Accepted or InviteState.RevokedByTenant)
		{
			throw new InvalidOperationException($"Cannot send invite in status '{State}'.");
		}

		if (IsExpired(nowUtc))
		{
			throw new InvalidOperationException("Cannot send an expired invite.");
		}

		State = InviteState.Sent;
		SentAtUtc = nowUtc;
		SendCount++;
	}

	public void Accept(AccountId accountId, DateTimeOffset nowUtc)
	{
		if (accountId == AccountId.Empty)
		{
			throw new ArgumentException("AccountId is required.", nameof(accountId));
		}

		if (State is InviteState.RevokedByTenant)
		{
			throw new InvalidOperationException("Invite revoked.");
		}

		if (IsExpired(nowUtc))
		{
			throw new InvalidOperationException("Invite expired.");
		}

		if (State is InviteState.Accepted)
		{
			throw new InvalidOperationException("Invite already accepted.");
		}

		State = InviteState.Accepted;
		AcceptedAtUtc = nowUtc;
		AcceptedAccountId = accountId;
	}

	public void Revoke(AccountId revokedByAccountId, DateTimeOffset nowUtc)
	{
		if (revokedByAccountId == AccountId.Empty)
		{
			throw new ArgumentException("RevokedByAccountId is required.", nameof(revokedByAccountId));
		}

		if (State is InviteState.Accepted)
		{
			throw new InvalidOperationException("Cannot revoke an accepted invite.");
		}

		State = InviteState.RevokedByTenant;
		RevokedAtUtc = nowUtc;
		RevokedByAccountId = revokedByAccountId;
	}

	public bool IsExpired(DateTimeOffset nowUtc) => nowUtc >= ExpiresAtUtc && State != InviteState.Accepted;
}
