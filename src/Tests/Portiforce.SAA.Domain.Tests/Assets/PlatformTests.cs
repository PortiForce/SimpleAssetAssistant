using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Assets;

public sealed class PlatformTests
{
	[Fact]
	public void Rename_ShouldTrimName()
	{
		var p = Platform.Create("  Binance  ", "BINANCE", PlatformKind.Exchange);

		p.Rename("  Binance US  ");
		p.Name.Should().Be("Binance US");
	}

	[Fact]
	public void Rename_WhenReadonly_ShouldThrow()
	{
		var p = Platform.Create("Binance", "BINANCE", PlatformKind.Exchange, state: PlatformState.ReadOnly);

		var act = () => p.Rename("New Name");
		act.Should().Throw<DomainValidationException>();
	}
}