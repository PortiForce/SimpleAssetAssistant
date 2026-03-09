using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Assets;

public sealed class AssetCodeTests
{
	[Fact]
	public void Create_ShouldNormalizeToUpperInvariant()
	{
		var code = AssetCode.Create("  btc ");
		code.Value.Should().Be("BTC");
		code.ToString().Should().Be("BTC");
	}

	[Theory]
	[InlineData("")]

// space only
	[InlineData("   ")]
	[InlineData("B T C")]
	[InlineData("BTC!")]
	public void Create_WhenInvalid_ShouldThrow(string raw)
	{
		var act = () => AssetCode.Create(raw);
		act.Should().Throw<ArgumentException>();
	}
}