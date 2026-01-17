using Portiforce.SimpleAssetAssistant.Core.Extensions;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Internal;

namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct ActivityId(Guid Value)
{
	public static ActivityId New() => new(GuidIdUtil.New());
	public static ActivityId Empty => new(Guid.Empty);
	public static ActivityId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidIdUtil.ToString(Value);

	/// <summary>
	/// Parse requires non-empty GUID to avoid silently accepting "00000000-..."
	/// </summary>
	/// <param name="raw">string to parse</param>
	/// <returns>entity Identifier</returns>
	/// <exception cref="FormatException">In case provided value is not recognized as not empty Guid</exception>
	public static ActivityId Parse(string raw)
	{
		if (Guid.TryParse(raw, out var g) && g != Guid.Empty)
		{
			return From(g);
		}

		throw new FormatException(
			$"Value is not a valid non-empty {nameof(ActivityId)} GUID. Input (truncated): '{raw.Truncate()}'");
	}

	/// <summary>
	/// Attempts to parse the specified string representation of an activity identifier.
	/// </summary>
	/// <param name="raw">The string to parse as an activity identifier. Can be null.</param>
	/// <param name="id">When this method returns, contains the parsed <see cref="ActivityId"/> value if the parse operation succeeded, or
	/// <see cref="Empty"/> if it failed.</param>
	/// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
	public static bool TryParse(string? raw, out ActivityId id)
	{
		if (GuidIdUtil.TryParse(raw, out var g))
		{
			id = From(g);
			return true;
		}

		id = Empty;
		return false;
	}
}