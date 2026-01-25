namespace Portiforce.SimpleAssetAssistant.Core.Primitives;

/// <summary>
/// Domain Unit amount representation (only increments or positive values)
/// </summary>
public sealed record Quantity
{
	// Private Empty Constructor for EF Core
	private Quantity()
	{

	}

	public static Quantity Zero => new(0m);

	public decimal Value { get; init; }

	public bool IsZero => Value == 0m;
	public bool IsPositive => Value > 0m;

	private Quantity(decimal value)
	{
		Value = value;
	}

	public static Quantity Create(decimal value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
		return new Quantity(value);
	}

	public QuantityDelta ToDelta() => new(Value);
}