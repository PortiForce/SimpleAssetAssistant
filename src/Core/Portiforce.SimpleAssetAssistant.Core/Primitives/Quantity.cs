namespace Portiforce.SimpleAssetAssistant.Core.Primitives;

/// <summary>
/// Domain Unit amount representation
/// </summary>
public readonly record struct Quantity
{
	public static Quantity Zero => new(0m);

	public decimal Value { get; }

	public Quantity(decimal value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
		Value = value;
	}
}