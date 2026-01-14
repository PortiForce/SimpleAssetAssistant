namespace Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

public enum MovementRole : byte
{
	/// <summary>
	///  Main movement (buy/sell/exchange/transfer)
	/// </summary>
	Principal = 1,

	/// <summary>
	/// Fee leg associated with the main movement
	/// </summary>
	Fee = 2,

	/// <summary>
	/// Futures funding payment (optional, can be later)
	/// </summary>
	Funding = 3,

	/// <summary>
	/// Margin interest (optional)
	/// </summary>
	Interest = 4
}

