namespace Portiforce.SAA.Core.Primitives;

public sealed record FiatCurrency
{
	// Private Empty Constructor for EF Core
	private FiatCurrency()
	{

	}

	public string Code { get; init; } = null!;

	private FiatCurrency(string code) => Code = code;

	public static FiatCurrency Create(string rawData)
	{
		if (string.IsNullOrWhiteSpace(rawData))
		{
			throw new ArgumentException("Currency code cannot be empty.", nameof(rawData));
		}

		var code = rawData.Trim().ToUpperInvariant();

		// ISO 4217 codes are 3 letters; keep validation minimal + deterministic
		if (code.Length != 3 || !code.All(char.IsLetter))
		{
			throw new ArgumentException("Currency code must be a 3-letter ISO 4217 code.", nameof(rawData));
		}

		return new FiatCurrency(code);
	}

	public override string ToString() => Code;

	// Handy predefined values for common defaults (optional)
	public static readonly FiatCurrency USD = new("USD");
	public static readonly FiatCurrency EUR = new("EUR");
	public static readonly FiatCurrency GBP = new("GBP");
	public static readonly FiatCurrency UAH = new("UAH");
}
