namespace Portiforce.SAA.Core.Assets.Enums;

public enum PlatformAccountState : byte
{
	/// <summary>
	/// Account is defined but not yet linked or verified with the platform.
	/// </summary>
	Pending = 1,

	/// <summary>
	/// Fully active and usable: imports, aggregation, and reporting allowed.
	/// </summary>
	Active = 2,

	/// <summary>
	/// Temporarily disabled by user or system.
	/// </summary>
	Suspended = 3,

	/// <summary>
	/// Read-only mode: historical data available, but no new imports or sync allowed.
	/// </summary>
	ReadOnly = 4,

	/// <summary>
	/// Account is scheduled for removal but still retained for data consistency and export.
	/// </summary>
	PendingDeletion = 5,

	/// <summary>
	/// Soft-deleted; no longer visible in normal operations.
	/// </summary>
	Deleted = 6
}
