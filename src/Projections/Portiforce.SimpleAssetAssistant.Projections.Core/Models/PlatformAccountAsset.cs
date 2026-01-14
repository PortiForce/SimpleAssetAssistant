using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Projections.Core.Models;

public sealed class PlatformAccountAsset
{
	/// <summary>
	/// decomposition - platform to which this asset belongs
	/// </summary>
	public PlatformId PlatformId { get; init; }

	/// <summary>
	/// platform account to which this asset belongs
	/// </summary>
	public PlatformAccountId PlatformAccountId { get; set; }

	/// <summary>
	/// asset associated with this platform account
	/// </summary>
	public AssetId AssetId { get; init; }

	/// <summary>
	/// amount of assets related to this account
	/// </summary>
	public Quantity Amount { get; init; }
}
