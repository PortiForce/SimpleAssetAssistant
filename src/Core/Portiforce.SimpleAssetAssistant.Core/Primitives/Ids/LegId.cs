using Portiforce.SimpleAssetAssistant.Core.Extensions;

namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct LegId(Guid Value)
{
	public static LegId New() => new(GuidExtensions.New());
	public static LegId Empty => new(Guid.Empty);
	public static LegId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	/// <summary>
	/// Parse requires non-empty GUID to avoid silently accepting "00000000-..."
	/// </summary>
	/// <param name="raw">string to parse</param>
	/// <returns>entity Identifier</returns>
	/// <exception cref="FormatException">In case provided value is not recognized as not empty Guid</exception>
	public static LegId Parse(string raw)
	{
		if (Guid.TryParse(raw, out var g) && g != Guid.Empty)
		{
			return From(g);
		}

		throw new FormatException(
			$"Value is not a valid non-empty {nameof(LegId)} GUID. Input (truncated): '{raw.Truncate()}'");
	}

	/// <summary>
	/// Attempts to parse the specified string representation of a tenant identifier.
	/// </summary>
	/// <param name="raw">The string to parse as a tenant identifier. Can be null.</param>
	/// <param name="id">When this method returns, contains the parsed <see cref="LegId"/> value if the parse operation succeeded, or
	/// <see cref="Empty"/> if it failed.</param>
	/// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
	public static bool TryParse(string? raw, out LegId id)
	{
		if (GuidExtensions.TryParse(raw, out var g))
		{
			id = From(g);
			return true;
		}

		id = Empty;
		return false;
	}
}

