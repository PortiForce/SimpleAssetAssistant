namespace Portiforce.SAA.Contracts.UiSetup;

public static class UiPolicies
{
	public const string PlatformOwner = "PlatformOwner";
	public const string PlatformAdmin = "PlatformAdmin";
	public const string TenantAdmin = "TenantAdmin";
	public const string TenantUser = "TenantUser";

	// platform area
	public const string ViewTenants = "ViewTenants";
	public const string ManageTenants = "ManageTenants";
	public const string ViewTenantInfo = "ViewTenantInfo";
	public const string ViewTenantStats = "ViewTenantStats";

	// tenant admin area
	public const string InviteUsers = "InviteUsers";
	public const string ViewUsers = "ViewUsers";
	public const string ManageUsers = "ManageUsers";
	public const string ViewUserInfo = "ViewUserInfo";
	public const string ManageUserInfo = "ManageUserInfo";
	public const string ManagePlatformRestrictions = "ManagePlatformRestrictions";
	public const string ManageAssetRestrictions = "ManageAssetRestrictions";

	// tenant user area
	public const string ViewPortfolio = "ViewPortfolio";
	public const string ViewAssetInfo = "ViewAssetInfo";
	public const string ImportBulkStats = "ImportBulkStats";
	public const string ImportStats = "ImportStats";
	public const string ManageProfile = "ManageProfile";
}
