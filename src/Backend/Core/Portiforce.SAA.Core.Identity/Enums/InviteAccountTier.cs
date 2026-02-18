namespace Portiforce.SAA.Core.Identity.Enums;

public enum InviteAccountTier : byte
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
}
