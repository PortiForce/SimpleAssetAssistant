namespace Portiforce.SAA.Core.Assets.Enums;

public enum AssetKind
{
	Other = 0,

	/// <summary>
	/// Fiat currencies
	/// </summary>
	Fiat = 1,

	/// <summary>
	/// Common/Preferred shares
	/// </summary>
	Stock = 2,

	/// <summary>
	/// ETFs, Mutual Funds, and Index Funds
	/// </summary>
	Fund = 3,

	///<summary>
	///Gold, Silver, Oil
	///</summary>
	Commodity = 4,

	/// <summary>
	/// Digital assets (BTC, ETH)
	/// </summary>
	Crypto = 5,

	/// <summary>
	/// Pegged digital assets (USDC, USDT)
	/// </summary>
	Stablecoin = 6,
}