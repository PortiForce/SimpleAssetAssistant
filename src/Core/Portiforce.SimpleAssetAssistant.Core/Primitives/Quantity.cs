namespace Portiforce.SimpleAssetAssistant.Core.Primitives;

/// <summary>
/// Domain Unit amount representation (only increments or positive values)
/// </summary>
public readonly record struct Quantity
{
	public static Quantity Zero => new(0m);

	public decimal Value { get; }

	public bool IsZero => Value == 0m;
	public bool IsPositive => Value > 0m;

	public Quantity(decimal value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
		Value = value;
	}

	public QuantityDelta ToDelta() => new(Value);
}