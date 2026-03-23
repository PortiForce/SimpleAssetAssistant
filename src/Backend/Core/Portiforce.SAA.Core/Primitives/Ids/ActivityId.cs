using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct ActivityId(Guid Value) : IGuidId<ActivityId>
{
	public static ActivityId New() => new(GuidExtensions.New());
	public static ActivityId Empty => new(Guid.Empty);
	public static ActivityId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	public static ActivityId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(ActivityId));

	public static bool TryParse(string? raw, out ActivityId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);
}
