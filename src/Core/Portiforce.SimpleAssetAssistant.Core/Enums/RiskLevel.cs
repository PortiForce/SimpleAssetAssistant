namespace Portiforce.SimpleAssetAssistant.Core.Enums;

public enum RiskLevel : byte
{
	// 1. Safety first. Mostly Cash, Gov Bonds, Gold. 
	// Target: Preservation of Capital.
	Conservative = 1,

	// 2. Low risk. Blue-chip stocks + Bonds.
	// Target: Beating inflation with low volatility.
	ModeratelyConservative = 2,

	// 3. Balanced. 60/40 Stocks/Bonds or broad ETFs (S&P 500).
	// Target: Steady long-term growth.
	Moderate = 3,

	// 4. Growth focused. Tech stocks, Emerging Markets, small % Crypto.
	// Target: High returns, willing to accept market swings.
	ModeratelyAggressive = 4,

	// 5. Maximum growth. Crypto, Startups, Options, Leverage.
	// Target: Moonshot returns, high risk of principal loss.
	Aggressive = 5
}
