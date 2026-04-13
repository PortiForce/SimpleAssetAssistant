namespace Portiforce.SAA.Core.Identity.Enums;

public enum AccountState : byte
{
	/// <summary>
	///     Default zero value (not yet assigned)
	/// </summary>
	None = 0,

	/// <summary>
	///     Account state is unknown or undetermined
	/// </summary>
	Unknown = 1,

	/// <summary>
	///     Account is created but not yet verified/activated
	/// </summary>
	PendingActivation = 2,

	/// <summary>
	///     Account is active
	/// </summary>
	Active = 3,

	/// <summary>
	///     Account has been suspended (reason is TBD), but reversible
	/// </summary>
	Suspended = 4,

	/// <summary>
	///     Account is permanently disabled
	/// </summary>
	Disabled = 5,

	/// <summary>
	///     Account is scheduled for deletion (grace period) but still recoverable
	/// </summary>
	PendingDeletion = 6,

	/// <summary>
	///     Account is soft-deleted
	/// </summary>
	Deleted = 7
}