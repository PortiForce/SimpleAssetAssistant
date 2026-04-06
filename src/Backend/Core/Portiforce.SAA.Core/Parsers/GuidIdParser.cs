using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Core.Parsers;

public static class GuidIdParser
{
	/// <summary>
	///     Parses the specified string as a non-empty GUID and creates an identifier of type TId using the provided factory
	///     function.
	/// </summary>
	/// <typeparam name="TId">The type of the identifier to create from the parsed GUID.</typeparam>
	/// <param name="raw">The string representation of the GUID to parse. Must represent a valid, non-empty GUID.</param>
	/// <param name="factory">A function that creates an instance of TId from a parsed GUID.</param>
	/// <param name="typeName">The display name of the identifier type, used in exception messages.</param>
	/// <returns>An instance of TId created from the parsed GUID.</returns>
	/// <exception cref="FormatException">Thrown if raw does not represent a valid, non-empty GUID.</exception>
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
	///     Attempts to parse the specified string as a non-empty GUID and creates an identifier of type TId using the provided
	///     factory function.
	/// </summary>
	/// <remarks>
	///     This method does not throw an exception if parsing fails. The factory function is only invoked if
	///     the input string is a valid, non-empty GUID.
	/// </remarks>
	/// <typeparam name="TId">The type of the identifier to create from the parsed GUID.</typeparam>
	/// <param name="raw">The string representation of the GUID to parse. Can be null.</param>
	/// <param name="factory">A function that creates an identifier of type TId from a parsed GUID.</param>
	/// <param name="empty">The value to assign to id if parsing fails or the GUID is empty.</param>
	/// <param name="id">
	///     When this method returns, contains the identifier created from the parsed GUID if parsing succeeds and the GUID is
	///     not empty; otherwise, contains the value of empty.
	/// </param>
	/// <returns>true if raw is successfully parsed as a non-empty GUID and an identifier is created; otherwise, false.</returns>
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