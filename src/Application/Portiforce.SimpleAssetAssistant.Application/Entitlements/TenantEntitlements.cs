namespace Portiforce.SimpleAssetAssistant.Application.Entitlements;

public sealed record TenantEntitlements(
	int MaxUsers,
	int MaxPlatforms,
	int MaxDistinctAssets,
	int MaxImportRows,
	bool AllowProjections,
	bool AllowAdvancedAnalytics
);