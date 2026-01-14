namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct AssetId(Guid Value)
{
	public static AssetId New() => new(Guid.CreateVersion7());
	public static AssetId Empty => new(Guid.Empty);
	public static AssetId From(Guid value) => new(value);

	public static bool TryParse(string? raw, out AssetId id)
	{
		id = Empty;
		return Guid.TryParse(raw, out var g) && (id = From(g)) is var _;
	}

	public bool IsEmpty => Value == Guid.Empty;

	public override string ToString() => Value.ToString();
}
