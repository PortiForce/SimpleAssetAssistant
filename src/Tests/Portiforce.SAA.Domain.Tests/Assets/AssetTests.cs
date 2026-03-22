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
		var act = () => BuildTestAssetWithRawCode(AssetId.New(), state: AssetLifecycleState.Deleted);

		act.Should().Throw<DomainValidationException>();
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Create_WhenCodeIsEmpty_ShouldThrow(string code)
	{
		var act = () => BuildTestAssetWithRawCode(AssetId.New(), code: code);

		act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Create_WhenAssetCodeIsEmpty_ShouldThrow()
	{
		var act = () => BuildTestAssetWithAssetCode(AssetId.New(), null);

		var exceptionAssertions = act.Should().Throw<DomainValidationException>();

		exceptionAssertions.Which.Message.Should().Contain("AssetCode");
	}

	[Fact]
	public void Rename_ShouldTrimAndSetName()
	{
		var asset = Asset.Create(
			AssetCode.Create("BTC"),
			AssetKind.Crypto,
			AssetLifecycleState.Active,
			name: "Bitcoin");

		asset.Rename("  Bitcoin  ");
		asset.Name.Should().Be("Bitcoin");
	}

	[Theory]
	[InlineData(AssetLifecycleState.ReadOnly)]
	[InlineData(AssetLifecycleState.Disabled)]
	public void Rename_WhenReadonly_ShouldThrow(AssetLifecycleState state)
	{
		var asset = BuildTestAssetWithRawCode(AssetId.New(), state: state);

		var act = () => asset.Rename("New Name");
		act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void RegisterSynonym_WhenSameAsCode_ShouldReturnFalse()
	{
		var asset = BuildTestAssetWithRawCode(AssetId.New(), code: "BTC");

		var ok = asset.RegisterSynonym(AssetCode.Create("BTC"));

		ok.Should().BeFalse();
		asset.GetSynonyms().Should().BeEmpty();
	}

	[Fact]
	public void RegisterSynonym_WhenNew_ShouldBeAddedOnce()
	{
		var asset = BuildTestAssetWithRawCode(AssetId.New());

		asset.RegisterSynonym(AssetCode.Create("XBT")).Should().BeTrue();
		asset.RegisterSynonym(AssetCode.Create("XBT")).Should().BeFalse();

		asset.GetSynonyms().Select(x => x.Value).Should().BeEquivalentTo(["XBT"]);
	}

	[Theory]
	[InlineData(AssetLifecycleState.Active)]
	[InlineData(AssetLifecycleState.Archived)]
	public void Deactivate_WhenActive_ShouldSwitchToDisabled_AndSecondCallReturnsFalse(AssetLifecycleState state)
	{
		var asset = BuildTestAssetWithRawCode(AssetId.New(), state: state);

		asset.Deactivate().Should().BeTrue();
		asset.State.Should().Be(AssetLifecycleState.Disabled);

		Action act = () => asset.Deactivate();

		act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("It is not possible to update Readonly entity*");
	}

	private Asset BuildTestAssetWithRawCode(
		AssetId id,
		string code = "BTC",
		AssetLifecycleState state = AssetLifecycleState.Active)
	{
		var asset = Asset.Create(
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
		var asset = Asset.Create(
			assetCode,
			AssetKind.Crypto,
			state,
			"btc",
			4,
			id);

		return asset;
	}
}