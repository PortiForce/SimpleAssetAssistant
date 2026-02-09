namespace Portiforce.SAA.Core.Enums;

/// <summary>
/// Defines the high-level lifecycle status of a domain entity.
/// Controls visibility, mutability, and persistence behavior across the system.
/// </summary>
public enum EntityLifecycleState : byte
{
	/// <summary>
	/// The default state. The entity is fully visible in the UI and can be modified 
	/// (subject to standard business rules).
	/// </summary>
	Active = 1,

	/// <summary>
	/// The entity is visible and included in calculations/reports, but cannot be modified.
	/// Used for historical records, frozen periods, or imported data that must remain static.
	/// </summary>
	ReadOnly = 2,

	/// <summary>
	/// The entity exists but is temporarily turned off. 
	/// It is excluded from selection lists (e.g., "New Trade" dropdowns) but can be re-activated.
	/// </summary>
	Disabled = 3,

	/// <summary>
	/// The entity is preserved for compliance or audit purposes but is hidden from standard daily operations.
	/// Typically used for data older than a specific retention threshold.
	/// </summary>
	Archived = 4,

	/// <summary>
	/// Soft-deleted. The entity is marked for removal and is invisible to the application 
	/// but remains in the database to maintain referential integrity (foreign keys).
	/// </summary>
	Deleted = 5
}
