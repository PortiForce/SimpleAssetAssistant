using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;

public static class TenantDataSeeder
{
	public static List<Tenant> BuildTenants()
	{
		List<Tenant> tenants = [];

		Tenant rootTenant = GetPortiforceTenant();
		Tenant demoTenant = GetDemoTenant();

		tenants.Add(rootTenant);
		tenants.Add(demoTenant);

		return tenants;
	}

	public static Tenant GetPortiforceTenant()
	{
		return Tenant.Create(
			"PortiForce",
			"PORTIFORCE",
			"PortiForce",
			"app",
			TenantState.Active,
			TenantPlan.Pro);
	}

	public static Tenant GetDemoTenant()
	{
		return Tenant.Create(
			"Demo",
			"DEMO",
			"demo",
			"demo",
			TenantState.Active);
	}
}