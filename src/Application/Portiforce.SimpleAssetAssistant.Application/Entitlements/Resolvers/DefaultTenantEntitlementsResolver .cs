using Portiforce.SimpleAssetAssistant.Application.Entitlements;
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

		TenantPlan.Basic => new TenantEntitlements(50, 10, 200, 10_000, true, false),
		TenantPlan.Advanced => new TenantEntitlements(200, 25, 1000, 25_000, true, true),
		TenantPlan.Pro => new TenantEntitlements(1000, 50, 10_000, 150_000, true, true),

		_ => throw new ArgumentOutOfRangeException(nameof(plan))
	};
}
