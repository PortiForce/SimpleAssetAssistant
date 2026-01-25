using Microsoft.Extensions.Primitives;

namespace Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

internal static class DbConstants
{
	public static class CommonSettings
	{
		public const string DateTimeDateOnlyFormat = "date";
		public const string DateTimeSecOnlyAccuracyFormat = "datetime2(0)";
	}

	public static class Domain
	{
		public const string DefaultSchemaName = "pf";

		public static class EntityNames
		{
			public const string TenantEntityName = "Tenant";
			public const string TenantRestrictedAssetEntityName = "TenantRestrictedAsset";
			public const string TenantRestrictedPlatformEntityName = "TenantRestrictedPlatform";

			public const string PlatformEntityName = "Platform";
			public const string PlatformAccountEntityName = "PlatformAccount";
			public const string AccountEntityName = "Account";

			public const string AssetEntityName = "Asset";

			public const string ActivityEntityName = "Activity";
			public const string ActivityLegEntityName = "ActivityLeg";
		}
	}
}