using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Core;

public sealed class QuantityDeltaTests
{
	[Theory]
	[InlineData(-0.0001)]
	[InlineData(-1)]
	[InlineData(-123.45)]
	public void ToQuantityNonNegative_WhenNegative_ShouldThrow(decimal value)
	{
		QuantityDelta delta = new(value);

		Func<Quantity> act = delta.ToQuantityNonNegative;

		_ = act.Should()
			.Throw<ArgumentOutOfRangeException>()
			.Which.ParamName.Should()
			.Be("Value");
	}

	[Fact]
	public void Constructor_WhenZero_ShouldSetFlagsCorrectly()
	{
		QuantityDelta delta = new(0m);

		_ = delta.Value.Should().Be(0m);
		_ = delta.IsZero.Should().BeTrue();
		_ = delta.IsPositive.Should().BeFalse();
		_ = delta.IsNegative.Should().BeFalse();
	}

	[Theory]
	[InlineData(0.0001)]
	[InlineData(1)]
	[InlineData(123.45)]
	public void Constructor_WhenPositive_ShouldSetFlagsCorrectly(decimal value)
	{
		QuantityDelta delta = new(value);

		_ = delta.Value.Should().Be(value);
		_ = delta.IsZero.Should().BeFalse();
		_ = delta.IsPositive.Should().BeTrue();
		_ = delta.IsNegative.Should().BeFalse();
	}

	[Theory]
	[InlineData(-0.0001)]
	[InlineData(-1)]
	[InlineData(-123.45)]
	public void Constructor_WhenNegative_ShouldSetFlagsCorrectly(decimal value)
	{
		QuantityDelta delta = new(value);

		_ = delta.Value.Should().Be(value);
		_ = delta.IsZero.Should().BeFalse();
		_ = delta.IsPositive.Should().BeFalse();
		_ = delta.IsNegative.Should().BeTrue();
	}

	[Fact]
	public void Zero_ShouldRepresentZeroDelta()
	{
		QuantityDelta zero = QuantityDelta.Zero;

		_ = zero.Value.Should().Be(0m);
		_ = zero.IsZero.Should().BeTrue();
		_ = zero.IsPositive.Should().BeFalse();
		_ = zero.IsNegative.Should().BeFalse();
	}

	[Fact]
	public void Zero_ShouldBeEqualToExplicitZero()
	{
		QuantityDelta a = QuantityDelta.Zero;
		QuantityDelta b = new(0m);

		_ = a.Should().Be(b);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Abs_WhenPositive_ShouldReturnSameValue()
	{
		QuantityDelta delta = new(10.5m);

		QuantityDelta result = delta.Abs();

		_ = result.Value.Should().Be(10.5m);
		_ = result.IsPositive.Should().BeTrue();
	}

	[Fact]
	public void Abs_WhenNegative_ShouldReturnPositiveValue()
	{
		QuantityDelta delta = new(-10.5m);

		QuantityDelta result = delta.Abs();

		_ = result.Value.Should().Be(10.5m);
		_ = result.IsPositive.Should().BeTrue();
		_ = result.IsNegative.Should().BeFalse();
	}

	[Fact]
	public void Abs_WhenZero_ShouldReturnZero()
	{
		QuantityDelta delta = QuantityDelta.Zero;

		QuantityDelta result = delta.Abs();

		_ = result.Value.Should().Be(0m);
		_ = result.IsZero.Should().BeTrue();
	}

	[Fact]
	public void ToQuantityNonNegative_WhenZero_ShouldReturnZeroQuantity()
	{
		QuantityDelta delta = new(0m);

		Quantity quantity = delta.ToQuantityNonNegative();

		_ = quantity.Value.Should().Be(0m);
		_ = quantity.IsZero.Should().BeTrue();
	}

	[Theory]
	[InlineData(0.0001)]
	[InlineData(1)]
	[InlineData(123.45)]
	public void ToQuantityNonNegative_WhenPositive_ShouldReturnQuantity(decimal value)
	{
		QuantityDelta delta = new(value);

		Quantity quantity = delta.ToQuantityNonNegative();

		_ = quantity.Value.Should().Be(value);
		_ = quantity.IsPositive.Should().BeTrue();
	}

	[Fact]
	public void Constructor_WhenSameValue_ShouldBeEqual()
	{
		QuantityDelta a = new(10.5m);
		QuantityDelta b = new(10.5m);

		_ = a.Should().Be(b);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Constructor_WhenDifferentValue_ShouldNotBeEqual()
	{
		QuantityDelta a = new(10m);
		QuantityDelta b = new(-10m);

		_ = a.Should().NotBe(b);
	}
}