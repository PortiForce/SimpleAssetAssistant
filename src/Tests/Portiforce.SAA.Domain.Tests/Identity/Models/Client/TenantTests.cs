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
		Tenant t = Tenant.Create(
			"  Acme Corp  ",
			"ACME",
			"Acme",
			"acme",
			TenantState.Active,
			TenantPlan.Demo,
			TenantSettings.Default());

		_ = t.Name.Should().Be("Acme Corp");
	}

	[Fact]
	public void Rename_WhenEmpty_ShouldThrow()
	{
		Tenant t = Tenant.Create("Acme", "ACME", "Acme", "acme");

		Action act = () => t.Rename("   ");
		_ = act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void UpdateRestrictedAssetList_ShouldAddUnique_AndRemove()
	{
		Tenant t = Tenant.Create("Acme", "ACME", "Acme", "acme");
		AssetId a1 = AssetId.New();
		AssetId a2 = AssetId.New();

		t.UpdateRestrictedAssetList([a1, a1, a2], true);

		_ = t.RestrictedAssets.Select(x => x.AssetId).Should().BeEquivalentTo([a1, a2]);

		t.UpdateRestrictedAssetList([a1], false);

		_ = t.RestrictedAssets.Select(x => x.AssetId).Should().BeEquivalentTo([a2]);
	}
}