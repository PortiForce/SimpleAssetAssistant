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

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenEmpty_ShouldThrow(string? raw)
	{
		Func<FiatCurrency> act = () => FiatCurrency.Create(raw!);
		_ = act.Should().Throw<ArgumentException>();
	}

	[Theory]
	[InlineData("US")]
	[InlineData("USDD")]
	[InlineData("U$D")]
	[InlineData("12A")]
	public void Create_WhenInvalidCode_ShouldThrow(string raw)
	{
		Func<FiatCurrency> act = () => FiatCurrency.Create(raw);
		_ = act.Should().Throw<ArgumentException>();
	}
}