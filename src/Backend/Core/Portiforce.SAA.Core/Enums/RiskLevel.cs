namespace Portiforce.SAA.Core.Enums;

public enum RiskLevel : byte
{
	/// <summary>
	///     1. Safety first. Mostly Cash, Gov Bonds, Gold.
	///     Target: Preservation of Capital.
	/// </summary>
	Conservative = 1,

	/// <summary>
	///     2. Low risk. Blue-chip stocks + Bonds.
	///     Target: Beating inflation with low volatility.
	/// </summary>
	ModeratelyConservative = 2,

	/// <summary>
	///     3. Balanced. 60/40 Stocks/Bonds or broad ETFs (S&P 500).
	///     Target: Steady long-term growth.
	/// </summary>
	Moderate = 3,

	/// <summary>
	///     4. Growth focused. Tech stocks, Emerging Markets, small % Crypto.
	///     Target: High returns, willing to accept market swings.
	/// </summary>
	ModeratelyAggressive = 4,

	/// <summary>
	///     5. Maximum growth. Crypto, Startups, Options, Leverage.
	///     Target: Moonshot returns, high risk of principal loss.
	/// </summary>
	Aggressive = 5
}