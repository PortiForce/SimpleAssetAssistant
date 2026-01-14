namespace Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

public enum Role : byte
{
	None = 0,

	/// <summary>
	/// Platform scoped role with full control over the tenant including billing and structure
	/// </summary>
	PlatformOwner = 1,

	/// <summary>
	/// Platform scoped role to allow to configure tenants and their structure
	/// </summary>
	PlatformAdmin = 2,

	/// <summary>
	/// Tenant scoped  role to configure tenant params and users
	/// </summary>
	TenantAdmin = 3,

	/// <summary>
	/// Tenant scoped role for user who is associated with a particular tenant in a multi-tenant system.
	/// </summary>
	TenantUser = 4
}