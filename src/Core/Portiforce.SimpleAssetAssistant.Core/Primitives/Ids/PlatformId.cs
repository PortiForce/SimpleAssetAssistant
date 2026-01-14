namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct PlatformId(Guid Value)
{
	public static PlatformId New() => new(Guid.CreateVersion7());
	public static PlatformId Empty => new(Guid.Empty);
	public static PlatformId From(Guid value) => new(value);

	public static bool TryParse(string? raw, out PlatformId id)
	{
		id = Empty;
		return Guid.TryParse(raw, out var g) && (id = From(g)) is var _;
	}

	public bool IsEmpty => Value == Guid.Empty;

	public override string ToString() => Value.ToString();
}