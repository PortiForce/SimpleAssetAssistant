namespace Portiforce.SAA.Core.Extensions;

public static class GuidIdParser
{
	/// <summary>
	/// Parse requires non-empty GUID to avoid silently accepting "00000000-..."
	/// </summary>
	/// <param name="raw">string to parse</param>
	/// <returns>entity Identifier</returns>
	/// <exception cref="FormatException">In case provided value is not recognized as not empty Guid</exception>
	public static TId Parse<TId>(
		string raw,
		Func<Guid, TId> factory,
		string typeName)
	{
		if (GuidExtensions.TryParse(raw, out Guid guid) && guid != Guid.Empty)
		{
			return factory(guid);
		}

		throw new FormatException(
			$"Value is not a valid non-empty {typeName} GUID. Input (truncated): '{raw.Truncate()}'");
	}

	/// <summary>
	/// Attempts to parse the specified string representation of an asset identifier.
	/// </summary>
	/// <param name="raw">The string to parse as an asset identifier. Can be null.</param>
	/// <param name="id">When this method returns, contains the parsed <see cref="AssetId"/> value if the parse operation succeeded, or
	/// <see cref="Empty"/> if it failed.</param>
	/// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
	public static bool TryParse<TId>(
		string? raw,
		Func<Guid, TId> factory,
		TId empty,
		out TId id)
	{
		if (GuidExtensions.TryParse(raw, out Guid guid) && guid != Guid.Empty)
		{
			id = factory(guid);
			return true;
		}

		id = empty;
		return false;
	}
}
