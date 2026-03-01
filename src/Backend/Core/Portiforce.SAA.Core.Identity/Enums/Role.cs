namespace Portiforce.SAA.Core.Identity.Enums;

public enum Role : byte
{
	None = 0,

	/// <summary>
	/// Tenant scoped role for user who is associated with a particular tenant in a multi-tenant system.
	/// </summary>
	TenantUser = 1,

	/// <summary>
	/// Tenant scoped  role to configure tenant params and users
	/// </summary>
	TenantAdmin = 2,

	/// <summary>
	/// Tenant scoped background worker role
	/// </summary>
	TenantBackground = 5,

	/// <summary>
	/// Platform scoped role to allow to configure tenants and their structure
	/// </summary>
	PlatformAdmin = 10,

	/// <summary>
	/// Platform scoped role with full control over the tenant including billing and structure
	/// </summary>
	PlatformOwner = 21,
}