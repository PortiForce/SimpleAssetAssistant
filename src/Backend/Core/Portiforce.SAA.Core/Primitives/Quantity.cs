namespace Portiforce.SAA.Core.Primitives;

/// <summary>
///     Domain Unit amount representation (only increments or positive values)
/// </summary>
public sealed record Quantity
{
	// Private Empty Constructor for EF Core
	private Quantity()
	{
	}

	private Quantity(decimal value)
	{
		this.Value = value;
	}

	public static Quantity Zero => new(0m);

	public decimal Value { get; init; }

	public bool IsZero => this.Value == 0m;

	public bool IsPositive => this.Value > 0m;

	public static Quantity Create(decimal value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value);
		return new Quantity(value);
	}

	public QuantityDelta ToDelta() => new(this.Value);
}