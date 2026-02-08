namespace Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

public enum AccountState : byte
{
	/// <summary>
	/// Default account state
	/// </summary>
	Unknown = 1,

	/// <summary>
	/// Account is created but not yet verified/activated
	/// </summary>
	PendingActivation = 1,

	/// <summary>
	/// Account is active
	/// </summary>
	Active = 2,

	/// <summary>
	/// Account has been suspended (reason is TBD), but reversible
	/// </summary>
	Suspended = 3,

	/// <summary>
	/// Account is permanently disabled
	/// </summary>
	Disabled = 4,

	/// <summary>
	/// Account is scheduled for deletion (grace period) but still recoverable
	/// </summary>
	PendingDeletion = 5,

	/// <summary>
	/// Account is soft-deleted
	/// </summary>
	Deleted = 6
}