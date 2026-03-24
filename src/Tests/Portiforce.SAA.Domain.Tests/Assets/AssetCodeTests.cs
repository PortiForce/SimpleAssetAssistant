using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Assets;

public sealed class AssetCodeTests
{
	[Fact]
	public void Create_ShouldNormalizeToUpperInvariant()
	{
		AssetCode code = AssetCode.Create("  btc ");
		_ = code.Value.Should().Be("BTC");
		_ = code.ToString().Should().Be("BTC");
	}

	[Theory]
	[InlineData("")]

	// space only
	[InlineData("   ")]
	[InlineData("B T C")]
	[InlineData("BTC!")]
	public void Create_WhenInvalid_ShouldThrow(string raw)
	{
		Func<AssetCode> act = () => AssetCode.Create(raw);
		_ = act.Should().Throw<ArgumentException>();
	}
}