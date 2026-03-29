namespace Portiforce.SAA.Core.Primitives;

/// <summary>
///     Domain delta amount representation, accepts negative values as well
/// </summary>
public sealed record QuantityDelta
{
	// Private Empty Constructor for EF Core
	private QuantityDelta()
	{
	}

	public QuantityDelta(decimal value)
	{
		this.Value = value;
	}

	public static QuantityDelta Zero => new(0m);

	public decimal Value { get; init; }

	public bool IsZero => this.Value == 0m;

	public bool IsPositive => this.Value > 0m;

	public bool IsNegative => this.Value < 0m;

	public QuantityDelta Abs() => new(decimal.Abs(this.Value));

	public Quantity ToQuantityNonNegative()
		=>
			this.Value >= 0m
				? Quantity.Create(this.Value)
				: throw new ArgumentOutOfRangeException(nameof(this.Value), "Delta is negative.");
}