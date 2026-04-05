using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Parsers;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct PlatformAccountId(Guid Value) : IGuidId<PlatformAccountId>
{
	public static PlatformAccountId New() => new(GuidExtensions.New());

	public static PlatformAccountId Empty => new(Guid.Empty);

	public static PlatformAccountId From(Guid value) => new(value);

	public bool IsEmpty => this.Value == Guid.Empty;

	public static PlatformAccountId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(PlatformAccountId));

	public static bool TryParse(string? raw, out PlatformAccountId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);

	public override string ToString() => GuidExtensions.ToString(this.Value);
}