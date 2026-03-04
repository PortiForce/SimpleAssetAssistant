using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Primitives;

public sealed record AssetCode
{
	// Private Empty Constructor for EF Core
	private AssetCode()
	{

	}

	public string Value { get; init; } = null!;

	private AssetCode(string value)
	{
		Value = value;
	}

	public static AssetCode Create(string rawData)
	{
		if (string.IsNullOrWhiteSpace(rawData))
		{
			throw new ArgumentException("Asset code cannot be empty.", nameof(rawData));
		}

		var normalized = Normalize(rawData);

		if (!IsValid(normalized))
		{
			throw new ArgumentException($"Invalid asset code '{rawData}'.", nameof(rawData));
		}

		return new AssetCode(normalized);
	}

	public override string ToString() => Value;

	private static string Normalize(string input)
		=> input.Trim().ToUpperInvariant();

	private static bool IsValid(string code)
	{
		// Pragmatic rules:
		// - 2–16 chars covers BTC, ETH, USDT, VUSA, BRK.B, etc.
		// - Allow letters, digits, dot, dash
		if (code.Length is < EntityConstraints.Domain.Asset.CodeMinLength or > EntityConstraints.Domain.Asset.CodeMaxLength)
		{
			return false;
		}

		foreach (var c in code)
		{
			if (!char.IsLetterOrDigit(c) && c != '.' && c != '-')
			{
				return false;
			}
		}

		return true;
	}
}
