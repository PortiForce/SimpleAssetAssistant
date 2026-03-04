using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;

public static class TenantDataSeeder
{
	public static List<Tenant> BuildTenants()
	{
		var tenants = new List<Tenant>();

		var rootTenant = GetPortiforceTenant();
		var demoTenant = GetDemoTenant();

		tenants.Add(rootTenant);
		tenants.Add(demoTenant);

		return tenants;
	}

	public static Tenant GetPortiforceTenant()
	{
		return Tenant.Create(
			name: "PortiForce",
			code: "PORTIFORCE",
			brandName: "PortiForce",
			domainPrefix: "app",
			state: TenantState.Active,
			plan: TenantPlan.Pro
		);
	}

	public static Tenant GetDemoTenant()
	{
		return Tenant.Create(
			name: "Demo",
			code: "DEMO",
			brandName: "demo",
			domainPrefix: "demo",
			state: TenantState.Active,
			plan: TenantPlan.Demo
		);
	}
}