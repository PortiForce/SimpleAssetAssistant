using Portiforce.SimpleAssetAssistant.Application.Interfaces.Resolvers;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

namespace Portiforce.SimpleAssetAssistant.Application.Entitlements.Resolvers;

public sealed class DefaultTenantEntitlementsResolver : ITenantEntitlementsResolver
{
	public TenantEntitlements Resolve(TenantPlan plan) => plan switch
	{
		TenantPlan.Demo => new TenantEntitlements(
			MaxUsers: 10,
			MaxPlatforms: 5,
			MaxDistinctAssets: 20,
			MaxImportRows: 5_000,
			AllowProjections: false,
			AllowAdvancedAnalytics: false),

		TenantPlan.Basic => new TenantEntitlements(
			MaxUsers: 50,
			MaxPlatforms: 10,
			MaxDistinctAssets: 200,
			MaxImportRows: 10_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: false),


		TenantPlan.Advanced => new TenantEntitlements(
			MaxUsers: 100,
			MaxPlatforms: 20,
			MaxDistinctAssets: 500,
			MaxImportRows: 12_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: false),

		TenantPlan.Pro => new TenantEntitlements(
			MaxUsers: 200,
			MaxPlatforms: 25,
			MaxDistinctAssets: 1000,
			MaxImportRows: 20_000,
			AllowProjections: true,
			AllowAdvancedAnalytics: true),

		_ => throw new ArgumentOutOfRangeException(nameof(plan))
	};
}
