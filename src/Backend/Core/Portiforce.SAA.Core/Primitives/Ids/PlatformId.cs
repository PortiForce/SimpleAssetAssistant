using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Parsers;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct PlatformId(Guid Value) : IGuidId<PlatformId>
{
	public static PlatformId New() => new(GuidExtensions.New());

	public static PlatformId Empty => new(Guid.Empty);

	public static PlatformId From(Guid value) => new(value);

	public bool IsEmpty => this.Value == Guid.Empty;

	public static PlatformId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(PlatformId));

	public static bool TryParse(string? raw, out PlatformId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);

	public override string ToString() => GuidExtensions.ToString(this.Value);
}