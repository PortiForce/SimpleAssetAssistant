using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct LegId(Guid Value) : IEquatable<LegId>
{
	public static LegId New() => new(GuidExtensions.New());
	public static LegId Empty => new(Guid.Empty);
	public static LegId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	public static LegId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(LegId));

	public static bool TryParse(string? raw, out LegId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);

}
