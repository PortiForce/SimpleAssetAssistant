using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives;

public sealed class QuantityTests
{
	[Theory]
	[InlineData(-0.0001)]
	[InlineData(-1)]
	[InlineData(-123.45)]
	public void Create_WhenNegative_ShouldThrow(decimal value)
	{
		Func<Quantity> act = () => Quantity.Create(value);

		_ = act.Should()
			.Throw<ArgumentOutOfRangeException>()
			.Which.ParamName.Should()
			.Be("value");
	}

	[Fact]
	public void Create_WhenZero_ShouldSucceed()
	{
		Quantity quantity = Quantity.Create(0m);

		_ = quantity.Value.Should().Be(0m);
		_ = quantity.IsZero.Should().BeTrue();
		_ = quantity.IsPositive.Should().BeFalse();
	}

	[Theory]
	[InlineData(0.0001)]
	[InlineData(1)]
	[InlineData(123.45)]
	public void Create_WhenPositive_ShouldSucceed(decimal value)
	{
		Quantity quantity = Quantity.Create(value);

		_ = quantity.Value.Should().Be(value);
		_ = quantity.IsZero.Should().BeFalse();
		_ = quantity.IsPositive.Should().BeTrue();
	}

	[Fact]
	public void Zero_ShouldRepresentZeroQuantity()
	{
		Quantity zero = Quantity.Zero;

		_ = zero.Value.Should().Be(0m);
		_ = zero.IsZero.Should().BeTrue();
		_ = zero.IsPositive.Should().BeFalse();
	}

	[Fact]
	public void Zero_ShouldBeEqualToCreatedZero()
	{
		Quantity a = Quantity.Zero;
		Quantity b = Quantity.Create(0m);

		_ = a.Should().Be(b);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenSameValue_ShouldBeEqual()
	{
		Quantity a = Quantity.Create(10.5m);
		Quantity b = Quantity.Create(10.5m);

		_ = a.Should().Be(b);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenDifferentValue_ShouldNotBeEqual()
	{
		Quantity a = Quantity.Create(10m);
		Quantity b = Quantity.Create(11m);

		_ = a.Should().NotBe(b);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(12.345)]
	public void ToDelta_ShouldPreserveValue(decimal value)
	{
		Quantity quantity = Quantity.Create(value);

		QuantityDelta delta = quantity.ToDelta();

		_ = delta.Value.Should().Be(value);
	}

	[Fact]
	public void ToDelta_WhenZero_ShouldReturnZeroDelta()
	{
		Quantity quantity = Quantity.Zero;

		QuantityDelta delta = quantity.ToDelta();

		_ = delta.Value.Should().Be(0m);
	}
}