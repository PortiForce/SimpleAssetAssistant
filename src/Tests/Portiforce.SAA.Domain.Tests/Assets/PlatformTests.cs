using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Assets;

public sealed class PlatformTests
{
	[Fact]
	public void Create_ShouldTrimNameAndCode()
	{
		Platform platform = Platform.Create("  Binance  ", "  BINANCE  ", PlatformKind.Exchange);

		_ = platform.Name.Should().Be("Binance");
		_ = platform.Code.Should().Be("BINANCE");
	}

	[Fact]
	public void Rename_ShouldTrimName()
	{
		Platform platform = Platform.Create("  Binance  ", "BINANCE", PlatformKind.Exchange);

		platform.Rename("  Binance US  ");

		_ = platform.Name.Should().Be("Binance US");
	}

	[Fact]
	public void Rename_WhenReadonly_ShouldThrow()
	{
		Platform platform = Platform.Create(
			"Binance",
			"BINANCE",
			PlatformKind.Exchange,
			PlatformState.ReadOnly);

		Action act = () => platform.Rename("New Name");

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*Readonly entity*");
	}

	[Fact]
	public void ChangeState_WhenEditable_ShouldUpdateState()
	{
		Platform platform = Platform.Create("Binance", "BINANCE", PlatformKind.Exchange);

		platform.ChangeState(PlatformState.Deleted);

		_ = platform.State.Should().Be(PlatformState.Deleted);
	}

	[Fact]
	public void ChangeState_WhenReadonly_ShouldThrow()
	{
		Platform platform = Platform.Create(
			"Binance",
			"BINANCE",
			PlatformKind.Exchange,
			PlatformState.ReadOnly);

		Action act = () => platform.ChangeState(PlatformState.Active);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*Readonly entity*");
	}
}