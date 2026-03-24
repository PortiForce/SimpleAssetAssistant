using FluentAssertions.Specialized;

using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Assets;

public sealed class AssetTests
{
	[Fact]
	public void Create_WhenStateDeleted_ShouldThrow()
	{
		Func<Asset> act = () => this.BuildTestAssetWithRawCode(AssetId.New(), state: AssetLifecycleState.Deleted);

		_ = act.Should().Throw<DomainValidationException>();
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Create_WhenCodeIsEmpty_ShouldThrow(string code)
	{
		Func<Asset> act = () => this.BuildTestAssetWithRawCode(AssetId.New(), code);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Create_WhenAssetCodeIsEmpty_ShouldThrow()
	{
		Func<Asset> act = () => this.BuildTestAssetWithAssetCode(AssetId.New(), null);

		ExceptionAssertions<DomainValidationException>? exceptionAssertions =
			act.Should().Throw<DomainValidationException>();

		_ = exceptionAssertions.Which.Message.Should().Contain("AssetCode");
	}

	[Fact]
	public void Rename_ShouldTrimAndSetName()
	{
		Asset asset = Asset.Create(
			AssetCode.Create("BTC"),
			AssetKind.Crypto,
			AssetLifecycleState.Active,
			"Bitcoin");

		asset.Rename("  Bitcoin  ");
		_ = asset.Name.Should().Be("Bitcoin");
	}

	[Theory]
	[InlineData(AssetLifecycleState.ReadOnly)]
	[InlineData(AssetLifecycleState.Disabled)]
	public void Rename_WhenReadonly_ShouldThrow(AssetLifecycleState state)
	{
		Asset asset = this.BuildTestAssetWithRawCode(AssetId.New(), state: state);

		Action act = () => asset.Rename("New Name");
		_ = act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void RegisterSynonym_WhenSameAsCode_ShouldReturnFalse()
	{
		Asset asset = this.BuildTestAssetWithRawCode(AssetId.New());

		bool ok = asset.RegisterSynonym(AssetCode.Create("BTC"));

		_ = ok.Should().BeFalse();
		_ = asset.GetSynonyms().Should().BeEmpty();
	}

	[Fact]
	public void RegisterSynonym_WhenNew_ShouldBeAddedOnce()
	{
		Asset asset = this.BuildTestAssetWithRawCode(AssetId.New());

		_ = asset.RegisterSynonym(AssetCode.Create("XBT")).Should().BeTrue();
		_ = asset.RegisterSynonym(AssetCode.Create("XBT")).Should().BeFalse();

		_ = asset.GetSynonyms().Select(x => x.Value).Should().BeEquivalentTo("XBT");
	}

	[Theory]
	[InlineData(AssetLifecycleState.Active)]
	[InlineData(AssetLifecycleState.Archived)]
	public void Deactivate_WhenActive_ShouldSwitchToDisabled_AndSecondCallReturnsFalse(AssetLifecycleState state)
	{
		Asset asset = this.BuildTestAssetWithRawCode(AssetId.New(), state: state);

		_ = asset.Deactivate().Should().BeTrue();
		_ = asset.State.Should().Be(AssetLifecycleState.Disabled);

		Action act = () => asset.Deactivate();

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("It is not possible to update Readonly entity*");
	}

	private Asset BuildTestAssetWithRawCode(
		AssetId id,
		string code = "BTC",
		AssetLifecycleState state = AssetLifecycleState.Active)
	{
		Asset asset = Asset.Create(
			AssetCode.Create(code),
			AssetKind.Crypto,
			state,
			"btc",
			4,
			id);

		return asset;
	}

	private Asset BuildTestAssetWithAssetCode(
		AssetId id,
		AssetCode assetCode,
		AssetLifecycleState state = AssetLifecycleState.Active)
	{
		Asset asset = Asset.Create(
			assetCode,
			AssetKind.Crypto,
			state,
			"btc",
			4,
			id);

		return asset;
	}
}