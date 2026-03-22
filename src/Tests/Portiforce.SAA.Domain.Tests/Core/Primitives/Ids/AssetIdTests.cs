using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class AssetIdTests
{
	[Fact]
	public void Parse_WhenEmptyGuid_ShouldThrow()
	{
		var act = () => AssetId.Parse(Guid.Empty.ToString());
		act.Should().Throw<FormatException>();
	}

	[Fact]
	public void TryParse_WhenNull_ShouldReturnFalse()
	{
		var ok = AssetId.TryParse(null, out var id);

		ok.Should().BeFalse();
		id.IsEmpty.Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenValidGuid_ShouldReturnTrue()
	{
		var g = Guid.NewGuid();

		var ok = AssetId.TryParse(g.ToString(), out var id);

		ok.Should().BeTrue();
		id.Value.Should().Be(g);
	}
}