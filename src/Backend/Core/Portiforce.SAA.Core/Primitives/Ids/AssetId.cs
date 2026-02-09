using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct AssetId(Guid Value)
{
	public static AssetId New() => new(GuidExtensions.New());
	public static AssetId Empty => new(Guid.Empty);
	public static AssetId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	/// <summary>
	/// Parse requires non-empty GUID to avoid silently accepting "00000000-..."
	/// </summary>
	/// <param name="raw">string to parse</param>
	/// <returns>entity Identifier</returns>
	/// <exception cref="FormatException">In case provided value is not recognized as not empty Guid</exception>
	public static AssetId Parse(string raw)
	{
		if (Guid.TryParse(raw, out var g) && g != Guid.Empty)
		{
			return From(g);
		}

		throw new FormatException(
			$"Value is not a valid non-empty {nameof(AssetId)} GUID. Input (truncated): '{raw.Truncate()}'");
	}

	/// <summary>
	/// Attempts to parse the specified string representation of an asset identifier.
	/// </summary>
	/// <param name="raw">The string to parse as an asset identifier. Can be null.</param>
	/// <param name="id">When this method returns, contains the parsed <see cref="AssetId"/> value if the parse operation succeeded, or
	/// <see cref="Empty"/> if it failed.</param>
	/// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
	public static bool TryParse(string? raw, out AssetId id)
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
