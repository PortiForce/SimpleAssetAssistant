namespace Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public readonly record struct AccountId(Guid Value)
{
	public static AccountId New() => new(Guid.CreateVersion7());
	public static AccountId Empty => new(Guid.Empty);
	public static AccountId From(Guid value) => new(value);

	public static bool TryParse(string? raw, out AccountId id)
	{
		id = Empty;
		return Guid.TryParse(raw, out var g) && (id = From(g)) is var _;
	}

	public bool IsEmpty => Value == Guid.Empty;

	public override string ToString() => Value.ToString();
}
