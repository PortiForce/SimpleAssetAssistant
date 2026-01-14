namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct PlatformAccountId(Guid Value)
{
	public static PlatformAccountId New() => new(Guid.CreateVersion7());
	public static PlatformAccountId Empty => new(Guid.Empty);
	public static PlatformAccountId From(Guid value) => new(value);

	public static bool TryParse(string? raw, out PlatformAccountId id)
	{
		id = Empty;
		return Guid.TryParse(raw, out var g) && (id = From(g)) is var _;
	}

	public bool IsEmpty => Value == Guid.Empty;

	public override string ToString() => Value.ToString();
}