using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;

namespace Portiforce.SimpleAssetAssistant.Core.Assets.Extensions;

public static class AssetExtensions
{
	public static bool IsFiatOrStableKind(AssetKind kind)
		=> kind is AssetKind.Fiat or AssetKind.Stablecoin;
}
