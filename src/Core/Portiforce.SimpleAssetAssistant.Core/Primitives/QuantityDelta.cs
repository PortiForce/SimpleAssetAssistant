namespace Portiforce.SimpleAssetAssistant.Core.Primitives;

/// <summary>
/// Domain delta amount representation, accepts negative values as well 
/// </summary>
public readonly record struct QuantityDelta
{
	public static QuantityDelta Zero => new(0m);

	public decimal Value { get; }

	public bool IsZero => Value == 0m;
	public bool IsPositive => Value > 0m;
	public bool IsNegative => Value < 0m;

	public QuantityDelta(decimal value) => Value = value;

	public QuantityDelta Abs() => new(decimal.Abs(Value));

	public Quantity ToQuantityNonNegative()
		=> Value >= 0m ? Quantity.Create(Value)
			: throw new ArgumentOutOfRangeException(nameof(Value), "Delta is negative.");
}