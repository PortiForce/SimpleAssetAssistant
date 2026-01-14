namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct ActivityId(Guid Value)
{
	public static ActivityId New() => new(Guid.CreateVersion7());
	public static ActivityId Empty => new(Guid.Empty);
	public static ActivityId From(Guid value) => new(value);

	public static bool TryParse(string? raw, out ActivityId id)
	{
		id = Empty;
		return Guid.TryParse(raw, out var g) && (id = From(g)) is var _;
	}

	public bool IsEmpty => Value == Guid.Empty;

	public override string ToString() => Value.ToString();
}