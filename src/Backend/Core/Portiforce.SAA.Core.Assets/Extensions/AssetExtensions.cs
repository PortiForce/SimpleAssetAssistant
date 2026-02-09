using Portiforce.SAA.Core.Assets.Enums;

namespace Portiforce.SAA.Core.Assets.Extensions;

public static class AssetExtensions
{
	public static bool IsFiatOrStableKind(AssetKind kind)
		=> kind is AssetKind.Fiat or AssetKind.Stablecoin;
}
