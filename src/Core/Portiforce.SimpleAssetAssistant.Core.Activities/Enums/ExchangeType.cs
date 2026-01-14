namespace Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

public enum ExchangeType : byte
{
	/// <summary>
	/// Direct exchange between two assets (A → B),
	/// typically crypto-to-crypto or fiat-to-crypto.
	/// </summary>
	Direct = 1,

	/// <summary>
	/// Conversion performed by the platform as an internal operation,
	/// often without an explicit order book (e.g. "Convert" buttons).
	/// </summary>
	PlatformConversion = 2,

	/// <summary>
	/// Automatic exchange performed as part of another operation
	/// (e.g. auto-convert rewards, dust conversion).
	/// </summary>
	Automatic = 3
}
