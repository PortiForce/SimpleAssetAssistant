using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Assets;

public sealed class PlatformTests
{
	[Fact]
	public void Rename_ShouldTrimName()
	{
		Platform p = Platform.Create("  Binance  ", "BINANCE", PlatformKind.Exchange);

		p.Rename("  Binance US  ");
		_ = p.Name.Should().Be("Binance US");
	}

	[Fact]
	public void Rename_WhenReadonly_ShouldThrow()
	{
		Platform p = Platform.Create("Binance", "BINANCE", PlatformKind.Exchange, PlatformState.ReadOnly);

		Action act = () => p.Rename("New Name");
		_ = act.Should().Throw<DomainValidationException>();
	}
}