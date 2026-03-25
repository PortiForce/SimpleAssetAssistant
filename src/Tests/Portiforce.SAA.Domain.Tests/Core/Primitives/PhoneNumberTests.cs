using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives;

public sealed class PhoneNumberTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenEmpty_ShouldThrow(string? raw)
	{
		Func<PhoneNumber> act = () => PhoneNumber.Create(raw!);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("441234567890")]
	[InlineData("00441234567890")]
	[InlineData(" 441234567890 ")]
	public void Create_WhenDoesNotStartWithPlus_ShouldThrow(string raw)
	{
		Func<PhoneNumber> act = () => PhoneNumber.Create(raw);

		_ = act.Should().Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Fact]
	public void Create_WhenDigitCountIsBelow8_ShouldThrow()
	{
		Func<PhoneNumber> act = () => PhoneNumber.Create("+1234567");

		_ = act.Should().Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Fact]
	public void Create_WhenDigitCountIsAbove15_ShouldThrow()
	{
		Func<PhoneNumber> act = () => PhoneNumber.Create("+1234567890123456");

		_ = act.Should().Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("+")]
	[InlineData("+-")]
	[InlineData("+( )")]
	[InlineData("+abc")]
	public void Create_WhenNormalizationLeavesNoValidDigits_ShouldThrow(string raw)
	{
		Func<PhoneNumber> act = () => PhoneNumber.Create(raw);

		_ = act.Should().Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("+441234567890", "+441234567890")]
	[InlineData(" +44 1234 567890 ", "+441234567890")]
	[InlineData("+44-1234-567890", "+441234567890")]
	[InlineData("+44 (1234) 567890", "+441234567890")]
	public void Create_ShouldNormalizeToPlusAndDigitsOnly(string raw, string expected)
	{
		PhoneNumber phone = PhoneNumber.Create(raw);

		_ = phone.Value.Should().Be(expected);
		_ = phone.ToString().Should().Be(expected);
	}

	[Fact]
	public void Create_WhenDigitCountIsExactly8_ShouldSucceed()
	{
		PhoneNumber phone = PhoneNumber.Create("+12345678");

		_ = phone.Value.Should().Be("+12345678");
	}

	[Fact]
	public void Create_WhenDigitCountIsExactly15_ShouldSucceed()
	{
		PhoneNumber phone = PhoneNumber.Create("+123456789012345");

		_ = phone.Value.Should().Be("+123456789012345");
	}

	[Fact]
	public void TryCreate_WhenValid_ShouldReturnTrueAndNormalizedValue()
	{
		bool result = PhoneNumber.TryCreate(" +44 1234 567890 ", out PhoneNumber phone);

		_ = result.Should().BeTrue();
		_ = phone.Should().NotBeNull();
		_ = phone.Value.Should().Be("+441234567890");
	}

	[Fact]
	public void TryCreate_WhenInvalid_ShouldReturnFalseAndDefault()
	{
		bool result = PhoneNumber.TryCreate("123456789", out PhoneNumber phone);

		_ = result.Should().BeFalse();
		_ = phone.Should().BeNull();
	}

	[Fact]
	public void Create_WhenSameNormalizedValue_ShouldBeEqual()
	{
		PhoneNumber a = PhoneNumber.Create("+44 1234 567890");
		PhoneNumber b = PhoneNumber.Create("+441234567890");
		PhoneNumber c = PhoneNumber.Create("+44-1234-567890");

		_ = a.Should().Be(b);
		_ = b.Should().Be(c);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenDifferentValues_ShouldNotBeEqual()
	{
		PhoneNumber a = PhoneNumber.Create("+441234567890");
		PhoneNumber b = PhoneNumber.Create("+491234567890");

		_ = a.Should().NotBe(b);
	}
}