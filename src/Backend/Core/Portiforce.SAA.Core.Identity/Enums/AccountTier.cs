namespace Portiforce.SAA.Core.Identity.Enums;

public enum AccountTier : byte
{
	None = 0,

	/// <summary>
	/// Just looking, maybe 1 portfolio, delayed data.
	/// </summary>
	Observer = 1,

	/// <summary>
	/// Managing personal assets, simple goals.
	/// </summary>
	Investor = 2,

	/// <summary>
	/// Using projections, complex rebalancing.
	/// </summary>
	Strategist = 3,

	/// <summary>
	/// Strategist + API access, high limits.
	/// </summary>
	Institutional = 4
}
