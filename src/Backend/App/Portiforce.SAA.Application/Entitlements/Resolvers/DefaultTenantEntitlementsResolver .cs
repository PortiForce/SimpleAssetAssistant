using Portiforce.SAA.Application.Interfaces.Resolvers;
using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.Entitlements.Resolvers;

public sealed class DefaultTenantEntitlementsResolver : ITenantEntitlementsResolver
{
	// MaxPendingInvites: usually 2x MaxActiveUsers, but for demo we want to allow more invites to let users try the product with more people

	public TenantEntitlements Resolve(TenantPlan plan) => plan switch
	{
		TenantPlan.Demo => new TenantEntitlements(
			MaxActiveUsers: 10,
			MaxPendingInvites: 30,
			MaxPlatforms: 5,
			MaxDistinctAssets: 20,
			MaxImportRows: 5_000,
			AllowProjections: false,
			AllowAdvancedAnalytics: false),

		TenantPlan.Basic => new TenantEntitlements(
			MaxActiveUsers: 50,
			MaxPendingInvites: 100,
			MaxPlatforms: 10,
			MaxDistinctAssets: 200,
			MaxImportRows: 10_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: false),


		TenantPlan.Advanced => new TenantEntitlements(
			MaxActiveUsers: 100,
			MaxPendingInvites: 200,
			MaxPlatforms: 20,
			MaxDistinctAssets: 500,
			MaxImportRows: 12_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: false),

		TenantPlan.Pro => new TenantEntitlements(
			MaxActiveUsers: 200,
			MaxPendingInvites: 500,
			MaxPlatforms: 50,
			MaxDistinctAssets: 1000,
			MaxImportRows: 20_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: true),

		TenantPlan.Custom => new TenantEntitlements(
			MaxActiveUsers: 25,
			MaxPendingInvites: 50,
			MaxPlatforms: 50,
			MaxDistinctAssets: 1000,
			MaxImportRows: 20_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: true),

		_ => throw new ArgumentOutOfRangeException(nameof(plan))
	};
}
