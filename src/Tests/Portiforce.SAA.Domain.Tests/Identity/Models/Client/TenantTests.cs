using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Identity.Models.Client;

public sealed class TenantTests
{
	[Fact]
	public void Create_ShouldTrimAndValidateName()
	{
		var t = Tenant.Create(
			name: "  Acme Corp  ",
			code: "ACME",
			brandName: "Acme",
			domainPrefix: "acme",
			state: TenantState.Active,
			plan: TenantPlan.Demo,
			settings: TenantSettings.Default());

		t.Name.Should().Be("Acme Corp");
	}

	[Fact]
	public void Rename_WhenEmpty_ShouldThrow()
	{
		var t = Tenant.Create("Acme", "ACME", "Acme", "acme");

		var act = () => t.Rename("   ");
		act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void UpdateRestrictedAssetList_ShouldAddUnique_AndRemove()
	{
		var t = Tenant.Create("Acme", "ACME", "Acme", "acme");
		var a1 = AssetId.New();
		var a2 = AssetId.New();

		t.UpdateRestrictedAssetList([a1, a1, a2], isRestricted: true);

		t.RestrictedAssets.Select(x => x.AssetId).Should().BeEquivalentTo([a1, a2]);

		t.UpdateRestrictedAssetList([a1], isRestricted: false);

		t.RestrictedAssets.Select(x => x.AssetId).Should().BeEquivalentTo([a2]);
	}
}