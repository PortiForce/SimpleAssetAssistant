namespace Portiforce.SAA.Application.Entitlements;

public sealed record TenantEntitlements(
	int MaxActiveUsers,
	int MaxPendingInvites,
	int MaxPlatforms,
	int MaxDistinctAssets,
	int MaxImportRows,
	bool AllowProjections,
	bool AllowAdvancedAnalytics);