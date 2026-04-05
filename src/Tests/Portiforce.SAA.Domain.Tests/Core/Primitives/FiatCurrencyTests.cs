using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives;

public sealed class FiatCurrencyTests
{
	[Fact]
	public void Create_ShouldTrimAndUppercase()
	{
		FiatCurrency c = FiatCurrency.Create("  usd  ");
		_ = c.Code.Should().Be("USD");
		_ = c.ToString().Should().Be("USD");
	}

	[Fact]
	public void Create_WhenSameNormalizedCode_ShouldBeEqual()
	{
		FiatCurrency a = FiatCurrency.Create("usd");
		FiatCurrency b = FiatCurrency.Create("USD");
		FiatCurrency c = FiatCurrency.Create("  UsD ");

		_ = a.Should().Be(b);
		_ = b.Should().Be(c);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenEmpty_ShouldThrow(string? raw)
	{
		Func<FiatCurrency> act = () => FiatCurrency.Create(raw!);
		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("US")]
	[InlineData("USDD")]
	[InlineData("U$D")]
	[InlineData("12A")]
	public void Create_WhenInvalidCode_ShouldThrow(string raw)
	{
		Func<FiatCurrency> act = () => FiatCurrency.Create(raw);
		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("US1")]
	[InlineData("1SD")]
	[InlineData("U D")]
	[InlineData("U-D")]
	[InlineData("ÜSD")]
	public void Create_WhenCodeContainsNonAsciiLettersOrSymbols_ShouldThrow(string raw)
	{
		Func<FiatCurrency> act = () => FiatCurrency.Create(raw);
		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("USD")]
	[InlineData("EUR")]
	[InlineData("GBP")]
	[InlineData("UAH")]
	public void PredefinedValues_ShouldMatchFactory(string code)
	{
		FiatCurrency expected = FiatCurrency.Create(code);

		FiatCurrency actual = code switch
		{
			"USD" => FiatCurrency.USD,
			"EUR" => FiatCurrency.EUR,
			"GBP" => FiatCurrency.GBP,
			"UAH" => FiatCurrency.UAH,
			_ => throw new InvalidOperationException("Unsupported test code.")
		};

		_ = actual.Should().Be(expected);
		_ = actual.Code.Should().Be(code);
	}

	[Theory]
	[InlineData("usd", "USD")]
	[InlineData("Usd", "USD")]
	[InlineData("gBp", "GBP")]
	[InlineData(" uah ", "UAH")]
	public void Create_ShouldNormalizeInput(string raw, string expected)
	{
		FiatCurrency result = FiatCurrency.Create(raw);

		_ = result.Code.Should().Be(expected);
	}
}