namespace Portiforce.SAA.Core.Activities.Enums;


public enum AssetAllocationType : byte
{
	Spot = 1,

	/// <summary>
	/// Staking/earn (optional)
	/// </summary>
	Earn = 2,

	Futures = 10
}
