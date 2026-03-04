namespace Portiforce.SAA.Application.Entitlements;

public sealed record AccountEntitlements(
	int MaxPlatformsPerUser,
	int MaxAssetsVisible,
	bool AllowExports
);
