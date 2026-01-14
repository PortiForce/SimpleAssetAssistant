namespace Portiforce.SimpleAssetAssistant.Core.Assets.Enums;

public enum PlatformState : byte
{
	/// <summary>
	/// Fully operational
	/// </summary>
	Active = 1,

	/// <summary>
	/// Historical data only
	/// </summary>
	ReadOnly = 2,

	/// <summary>
	/// No longer supported (kept for history)
	/// </summary>
	Deprecated = 3,

	/// <summary>
	/// Soft deleted
	/// </summary>
	Deleted = 4
}
