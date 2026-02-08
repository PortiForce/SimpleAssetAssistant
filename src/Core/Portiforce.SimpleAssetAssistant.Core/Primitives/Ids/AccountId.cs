using Portiforce.SimpleAssetAssistant.Core.Extensions;

namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct AccountId(Guid Value)
{
	public static AccountId New() => new(GuidExtensions.New());
	public static AccountId Empty => new(Guid.Empty);
	public static AccountId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	/// <summary>
	/// Parse requires non-empty GUID to avoid silently accepting "00000000-..."
	/// </summary>
	/// <param name="raw">string to parse</param>
	/// <returns>entity Identifier</returns>
	/// <exception cref="FormatException">In case provided value is not recognized as not empty Guid</exception>
	public static AccountId Parse(string raw)
	{
		if (Guid.TryParse(raw, out var g) && g != Guid.Empty)
		{
			return From(g);
		}

		throw new FormatException(
			$"Value is not a valid non-empty {nameof(AccountId)} GUID. Input (truncated): '{raw.Truncate()}'");
	}

	/// <summary>
	/// Attempts to parse the specified string representation of an account identifier.
	/// </summary>
	/// <param name="raw">The string to parse as an account identifier. Can be null.</param>
	/// <param name="id">When this method returns, contains the parsed <see cref="AccountId"/> value if the parse operation succeeded, or
	/// <see cref="Empty"/> if it failed.</param>
	/// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
	public static bool TryParse(string? raw, out AccountId id)
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
