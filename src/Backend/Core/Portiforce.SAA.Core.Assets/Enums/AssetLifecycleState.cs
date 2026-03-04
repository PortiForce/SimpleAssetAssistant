namespace Portiforce.SAA.Core.Assets.Enums;

/// <summary>
/// Represents the specific lifecycle of an Asset definition.
/// Controls whether the asset can be used in new activities (Trades/Transfers) and how it appears in reports.
/// </summary>
public enum AssetLifecycleState : byte
{
	/// <summary>
	/// The asset has been identified (e.g., via bulk import) but is incomplete or unverified.
	/// <para>Behavior: <b>Hidden</b> from selection lists; <b>Cannot</b> be used in new Activities; <b>Mutable</b>.</para>
	/// </summary>
	Draft = 0,

	/// <summary>
	/// The standard state. The asset is fully defined and verified.
	/// <para>Behavior: <b>Visible</b> everywhere; <b>Usable</b> in Activities; <b>Mutable</b> (rename/update allowed).</para>
	/// </summary>
	Active = 1,

	/// <summary>
	/// The asset is valid but locked to prevent changes. often used for system/default assets or historic tickers that renamed.
	/// <para>Behavior: <b>Visible</b>; <b>Usable</b> (for history); <b>Immutable</b> (cannot rename).</para>
	/// </summary>
	ReadOnly = 2,

	/// <summary>
	/// The asset is temporarily turned off (e.g. delisted or user preference).
	/// <para>Behavior: <b>Hidden</b> from "New Trade" dropdowns; <b>Excluded</b> from current valuation; <b>Mutable</b>.</para>
	/// </summary>
	Disabled = 3,

	/// <summary>
	/// Long-term storage for assets no longer in circulation (e.g. liquidated companies).
	/// <para>Behavior: <b>Hidden</b> from standard UI; <b>Available</b> in historical audit reports only.</para>
	/// </summary>
	Archived = 4,

	/// <summary>
	/// Soft-deleted. Kept only to maintain referential integrity for database constraints.
	/// <para>Behavior: <b>Invisible</b> to the application layer.</para>
	/// </summary>
	Deleted = 5
}
