namespace Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

public enum TenantState : byte
{
	/// <summary>
	/// Tenant created, provisioning/configuration in progress.
	/// </summary>
	Provisioning = 1,

	/// <summary>
	/// Tenant is fully active.
	/// </summary>
	Active = 2,

	/// <summary>
	/// Tenant is temporarily suspended (billing/compliance).
	/// </summary>
	Suspended = 3,

	/// <summary>
	/// Tenant is in offboarding/grace period (export allowed, usage disabled).
	/// </summary>
	Offboarding = 4,

	/// <summary>
	/// Tenant is soft-deleted.
	/// </summary>
	Deleted = 5
}
