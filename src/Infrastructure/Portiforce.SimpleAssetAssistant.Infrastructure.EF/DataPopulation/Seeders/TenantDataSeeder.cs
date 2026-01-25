using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders;

public static class TenantDataSeeder
{
	public static List<Tenant> GetTenants()
	{
		var tenants = new List<Tenant>();

		var rootTenant = GetPortiForceTenant();
		var demoTenant = GetDemoTenant();

		tenants.Add(rootTenant);
		tenants.Add(demoTenant);

		return tenants;
	}

	public static Tenant GetPortiForceTenant()
	{
		return Tenant.Create(
			name: "PortiForce",
			code: "PORTIFORCE",
			brandName: "PortiForce",
			domainPrefix: "",
			state: TenantState.Active,
			plan: TenantPlan.Demo
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