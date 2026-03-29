using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives;

public sealed class AssetCodeTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenEmpty_ShouldThrow(string? raw)
	{
		Func<AssetCode> act = () => AssetCode.Create(raw!);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("A B")]
	[InlineData("A_B")]
	[InlineData("A/B")]
	[InlineData("A\\B")]
	[InlineData("A,B")]
	[InlineData("A:B")]
	[InlineData("A+B")]
	[InlineData("A#B")]
	[InlineData("$BTC")]
	[InlineData("(BTC)")]
	public void Create_WhenCodeContainsInvalidCharacters_ShouldThrow(string raw)
	{
		Func<AssetCode> act = () => AssetCode.Create(raw);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Fact]
	public void Create_WhenLengthIsBelowMin_ShouldThrow()
	{
		string raw = new('A', EntityConstraints.Domain.Asset.CodeMinLength - 1);

		Func<AssetCode> act = () => AssetCode.Create(raw);

		_ = act.Should().Throw<ArgumentException>()
			.Which.ParamName.Should().Be("rawData");
	}

	[Fact]
	public void Create_WhenLengthIsAboveMax_ShouldThrow()
	{
		string raw = new('A', EntityConstraints.Domain.Asset.CodeMaxLength + 1);

		Func<AssetCode> act = () => AssetCode.Create(raw);

		_ = act.Should().Throw<ArgumentException>()
			.Which.ParamName.Should().Be("rawData");
	}

	[Theory]
	[InlineData("btc", "BTC")]
	[InlineData(" eth ", "ETH")]
	[InlineData("brk.b", "BRK.B")]
	[InlineData("usd-coin", "USD-COIN")]
	[InlineData("vusa", "VUSA")]
	public void Create_ShouldNormalizeInput(string raw, string expected)
	{
		AssetCode result = AssetCode.Create(raw);

		_ = result.Value.Should().Be(expected);
		_ = result.ToString().Should().Be(expected);
	}

	[Theory]
	[InlineData("--")]
	[InlineData("..")]
	[InlineData(".-")]
	public void Create_WhenCodeHasNoLettersOrDigits_ShouldThrow(string raw)
	{
		Func<AssetCode> act = () => AssetCode.Create(raw);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Create_WhenLengthIsExactlyMin_ShouldSucceed()
	{
		string raw = new('A', EntityConstraints.Domain.Asset.CodeMinLength);

		AssetCode result = AssetCode.Create(raw);

		_ = result.Value.Should().Be(raw);
	}

	[Fact]
	public void Create_WhenLengthIsExactlyMax_ShouldSucceed()
	{
		string raw = new('A', EntityConstraints.Domain.Asset.CodeMaxLength);

		AssetCode result = AssetCode.Create(raw);

		_ = result.Value.Should().Be(raw);
	}

	[Theory]
	[InlineData("BTC")]
	[InlineData("ETH2")]
	[InlineData("BRK.B")]
	[InlineData("USD-COIN")]
	[InlineData("A1")]
	[InlineData("A-1")]
	[InlineData("A.1")]
	public void Create_WhenCodeContainsAllowedCharacters_ShouldSucceed(string raw)
	{
		AssetCode result = AssetCode.Create(raw);

		_ = result.Value.Should().Be(raw.Trim().ToUpperInvariant());
	}

	[Fact]
	public void Create_WhenSameNormalizedValue_ShouldBeEqual()
	{
		AssetCode a = AssetCode.Create("btc");
		AssetCode b = AssetCode.Create("BTC");
		AssetCode c = AssetCode.Create("  BtC ");

		_ = a.Should().Be(b);
		_ = b.Should().Be(c);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenDifferentValues_ShouldNotBeEqual()
	{
		AssetCode a = AssetCode.Create("BTC");
		AssetCode b = AssetCode.Create("ETH");

		_ = a.Should().NotBe(b);
	}
}