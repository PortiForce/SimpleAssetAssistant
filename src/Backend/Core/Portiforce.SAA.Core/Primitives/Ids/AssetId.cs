using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Parsers;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct AssetId(Guid Value) : IGuidId<AssetId>
{
	public bool IsEmpty => this.Value == Guid.Empty;

	public static AssetId Empty => new(Guid.Empty);

	public static AssetId New() => new(GuidExtensions.New());

	public static AssetId From(Guid value) => new(value);

	public static AssetId Parse(string raw) => GuidIdParser.Parse(raw, From, nameof(AssetId));

	public static bool TryParse(string? raw, out AssetId id) => GuidIdParser.TryParse(raw, From, Empty, out id);

	public override string ToString() => GuidExtensions.ToString(this.Value);
}