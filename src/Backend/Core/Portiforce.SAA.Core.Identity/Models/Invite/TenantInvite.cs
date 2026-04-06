using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Identity.Models.Invite;

public sealed class TenantInvite : Entity<Guid>, IAggregateRoot
{
	private TenantInvite(
		Guid id,
		TenantId tenantId,
		InviteTarget inviteTarget,
		AccountId invitedByAccountId,
		string alias,
		Role intendedRole,
		AccountTier intendedTier,
		byte[] tokenHash,
		DateTimeOffset createdAtUtc,
		DateTimeOffset expiresAtUtc)
		: base(id)
	{
		if (id == Guid.Empty)
		{
			throw new DomainValidationException("Invite Id must be defined.");
		}

		if (tenantId == TenantId.Empty)
		{
			throw new ArgumentException("TenantId is required.", nameof(tenantId));
		}

		if (invitedByAccountId == AccountId.Empty)
		{
			throw new ArgumentException("InvitedByAccountId is required.", nameof(invitedByAccountId));
		}

		if (expiresAtUtc <= createdAtUtc)
		{
			throw new ArgumentException("ExpiresAt must be in the future.", nameof(expiresAtUtc));
		}

		if (inviteTarget.IsEmpty)
		{
			throw new ArgumentException("InviteTarget is required.", nameof(inviteTarget));
		}

		if (string.IsNullOrWhiteSpace(alias))
		{
			throw new DomainValidationException("Alias is required.");
		}

		alias = alias.Trim();

		int min = EntityConstraints.Domain.Invite.AliasMinLength;
		int max = EntityConstraints.Domain.Invite.AliasMaxLength;
		if (alias.Length < min || alias.Length > max)
		{
			throw new DomainValidationException($"Alias must be {min}..{max} characters.");
		}

		if (tokenHash is null || tokenHash.Length != 32)
		{
			throw new DomainValidationException("tokenHash must be defined as 32 bytes");
		}

		this.TenantId = tenantId;
		this.InviteTarget = inviteTarget;
		this.InvitedByAccountId = invitedByAccountId;
		this.Alias = alias;
		this.IntendedRole = intendedRole;
		this.IntendedTier = intendedTier;
		this.TokenHash = tokenHash;
		this.CreatedAtUtc = createdAtUtc;
		this.ExpiresAtUtc = expiresAtUtc;
	}

	// Private Empty Constructor for EF Core
	private TenantInvite()
	{
	}

	public TenantId TenantId { get; private set; }

	public InviteTarget InviteTarget { get; private set; } = default!;

	/// <summary>
	///     Who invited + intended permission set
	/// </summary>
	public AccountId InvitedByAccountId { get; private set; }

	public Role IntendedRole { get; private set; }

	public AccountTier IntendedTier { get; private set; }

	public byte[] TokenHash { get; private set; } = default!;

	/// <summary>
	///     Gets the current lifecycle state of the invite.Lifecycle
	/// </summary>
	/// <remarks>
	///     Use this property to determine the invite's status, such as whether it has been created, sent,
	///     accepted, or expired. The value reflects the most recent state transition.
	/// </remarks>
	public InviteState State { get; private set; } = InviteState.Created;

	public DateTimeOffset CreatedAtUtc { get; private set; }

	public DateTimeOffset ExpiresAtUtc { get; }

	public DateTimeOffset? SentAtUtc { get; private set; }

	public int SendCount { get; private set; }

	public DateTimeOffset? UpdatedAtUtc { get; private set; }

	public AccountId? AcceptedAccountId { get; private set; }

	public AccountId? RevokedByAccountId { get; private set; }

	/// <summary>
	///     Gets alias value that is recognized by Tenant Admin so it is easy to navigate and reference same person using
	///     different invite targets
	/// </summary>
	public string Alias { get; private set; }

	public bool? BlockFutureInvites { get; private set; }

	public static TenantInvite Create(
		TenantId tenantId,
		InviteTarget inviteTarget,
		AccountId invitedByAccountId,
		string alias,
		Role intendedRole,
		AccountTier intendedTier,
		byte[] tokenHash,
		DateTimeOffset now,
		DateTimeOffset expiresAtUtc,
		Guid inviteId = default)
	{
		return new TenantInvite(
			inviteId == Guid.Empty ? GuidExtensions.New() : inviteId,
			tenantId,
			inviteTarget,
			invitedByAccountId,
			alias,
			intendedRole,
			intendedTier,
			tokenHash,
			now,
			expiresAtUtc);
	}

	public void MarkSent(DateTimeOffset nowUtc)
	{
		if (this.State is InviteState.Accepted or InviteState.RevokedByTenant)
		{
			throw new InvalidOperationException($"Cannot send invite in status '{this.State}'.");
		}

		if (this.IsExpired(nowUtc))
		{
			throw new InvalidOperationException("Cannot send an expired invite.");
		}

		if (this.BlockFutureInvites.HasValue && this.BlockFutureInvites.Value)
		{
			throw new InvalidOperationException("Cannot send an invite to blocked record.");
		}

		this.State = InviteState.Sent;
		this.SentAtUtc = nowUtc;
		this.SendCount++;
	}

	public bool Accept(AccountId accountId, DateTimeOffset nowUtc)
	{
		if (accountId == AccountId.Empty)
		{
			throw new ArgumentException("AccountId is required.", nameof(accountId));
		}

		if (this.State is InviteState.Accepted)
		{
			return false;
		}

		if (this.State is InviteState.RevokedByTenant)
		{
			throw new InvalidOperationException("Invite revoked.");
		}

		if (this.IsExpired(nowUtc))
		{
			throw new InvalidOperationException("Invite expired.");
		}

		this.State = InviteState.Accepted;
		this.UpdatedAtUtc = nowUtc;
		this.AcceptedAccountId = accountId;
		return true;
	}

	public bool Revoke(AccountId revokedByAccountId, DateTimeOffset nowUtc, bool blockFutureInvites)
	{
		if (revokedByAccountId == AccountId.Empty)
		{
			throw new ArgumentException("RevokedByAccountId is required.", nameof(revokedByAccountId));
		}

		if (this.State is InviteState.RevokedByTenant)
		{
			return false;
		}

		if (this.State is InviteState.Accepted)
		{
			throw new InvalidOperationException("Cannot revoke an accepted invite.");
		}

		if (this.State is InviteState.DeclinedByUser)
		{
			throw new InvalidOperationException("Cannot revoke declined invite.");
		}

		this.BlockFutureInvites = blockFutureInvites;
		this.State = InviteState.RevokedByTenant;
		this.UpdatedAtUtc = nowUtc;
		this.RevokedByAccountId = revokedByAccountId;
		return true;
	}

	public bool Decline(DateTimeOffset nowUtc)
	{
		if (this.State == InviteState.DeclinedByUser)
		{
			return false;
		}

		// decline action is more priority over revoke as it is expresses user's intent, while revoke is tenant's intent. So we allow decline even if the invite is revoked, but not the opposite.
		if (this.State is InviteState.Accepted)
		{
			throw new InvalidOperationException("Cannot decline an accepted invite.");
		}

		if (this.State is not InviteState.RevokedByTenant and not InviteState.Expired)
		{
			this.State = InviteState.DeclinedByUser;
		}

		// express intention of the person do no longer receive any invites
		this.BlockFutureInvites = true;
		this.UpdatedAtUtc = nowUtc;
		return true;
	}

	public bool IsExpired(DateTimeOffset nowUtc) => nowUtc >= this.ExpiresAtUtc && this.State != InviteState.Accepted;
}