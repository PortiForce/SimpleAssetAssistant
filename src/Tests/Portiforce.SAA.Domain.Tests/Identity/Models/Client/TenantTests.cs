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
		Tenant tenant = Tenant.Create(
			"  Acme Corp  ",
			"ACME",
			"Acme",
			"acme",
			TenantState.Active,
			TenantPlan.Demo,
			TenantSettings.Default());

		_ = tenant.Name.Should().Be("Acme Corp");
	}

	[Fact]
	public void Rename_WhenEmpty_ShouldThrow()
	{
		Tenant tenant = Tenant.Create("Acme", "ACME", "Acme", "acme", TenantState.Active, TenantPlan.Demo);

		Action act = () => tenant.Rename("   ");

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*Tenant name is required*");
	}

	[Fact]
	public void Rename_WhenDeleted_ShouldThrow()
	{
		Tenant tenant = Tenant.Create(
			"Acme",
			"ACME",
			"Acme",
			"acme",
			TenantState.Deleted,
			TenantPlan.Demo);

		Action act = () => tenant.Rename("New Name");

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*deleted*");
	}

	[Fact]
	public void UpdateRestrictedAssetList_ShouldAddUnique_AndRemove()
	{
		Tenant tenant = Tenant.Create("Acme", "ACME", "Acme", "acme", TenantState.Active, TenantPlan.Demo);
		AssetId a1 = AssetId.New();
		AssetId a2 = AssetId.New();

		tenant.UpdateRestrictedAssetList([a1, a1, a2], true);

		_ = tenant.RestrictedAssets.Select(x => x.AssetId).Should().BeEquivalentTo([a1, a2]);

		tenant.UpdateRestrictedAssetList([a1], false);

		_ = tenant.RestrictedAssets.Select(x => x.AssetId).Should().BeEquivalentTo([a2]);
	}

	[Fact]
	public void UpdateRestrictedPlatformList_ShouldAddUnique_AndRemove()
	{
		Tenant tenant = Tenant.Create("Acme", "ACME", "Acme", "acme", TenantState.Active, TenantPlan.Demo);
		PlatformId p1 = PlatformId.New();
		PlatformId p2 = PlatformId.New();

		tenant.UpdateRestrictedPlatformList([p1, p1, p2], true);

		_ = tenant.RestrictedPlatforms.Select(x => x.PlatformId).Should().BeEquivalentTo([p1, p2]);

		tenant.UpdateRestrictedPlatformList([p1], false);

		_ = tenant.RestrictedPlatforms.Select(x => x.PlatformId).Should().BeEquivalentTo([p2]);
	}

	[Fact]
	public void UpdateSettings_WhenNull_ShouldThrow()
	{
		Tenant tenant = Tenant.Create("Acme", "ACME", "Acme", "acme", TenantState.Active, TenantPlan.Demo);

		Action act = () => tenant.UpdateSettings(null!);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*Settings cannot be null*");
	}
}