using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives;

public sealed class FiatCurrencyTests
{
	[Fact]
	public void Create_ShouldTrimAndUppercase()
	{
		var c = FiatCurrency.Create("  usd  ");
		c.Code.Should().Be("USD");
		c.ToString().Should().Be("USD");
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenEmpty_ShouldThrow(string? raw)
	{
		var act = () => FiatCurrency.Create(raw!);
		act.Should().Throw<ArgumentException>();
	}

	[Theory]
	[InlineData("US")]
	[InlineData("USDD")]
	[InlineData("U$D")]
	[InlineData("12A")]
	public void Create_WhenInvalidCode_ShouldThrow(string raw)
	{
		var act = () => FiatCurrency.Create(raw);
		act.Should().Throw<ArgumentException>();
	}
}