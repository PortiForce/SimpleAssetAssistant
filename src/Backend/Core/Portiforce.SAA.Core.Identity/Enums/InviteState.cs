namespace Portiforce.SAA.Core.Identity.Enums;

public enum InviteState : byte
{
	Created = 0,

	/// <summary>
	/// Invite were processed by Notification system
	/// </summary>
	Sent = 1,

	/// <summary>
	/// invite successfully accepted
	/// </summary>
	Accepted = 2,

	/// <summary>
	/// Invite were revoked by Tenant
	/// </summary>
	RevokedByTenant = 3,

	/// <summary>
	/// Invite were declined by User (aka no longer disturb option)
	/// </summary>
	DeclinedByUser = 4,

	/// <summary>
	/// Edge case : invite were failed (unsuccessful during an accept attempt)
	/// </summary>
	AcceptAttemptFailed = 5,
}
