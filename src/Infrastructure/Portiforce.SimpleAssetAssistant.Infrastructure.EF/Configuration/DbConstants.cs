
namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

internal static class DbConstants
{
	public static class CommonSettings
	{
		public const string DateTimeDateOnlyFormat = "date";
		public const string DateTimeSecOnlyAccuracyFormat = "datetime2(0)";

		public const string VarBinaryDataType = "varbinary(max)";
		public const string Varbinary32DataType = "varbinary(32)";
	}

	public static class Domain
	{
		public static class Entities
		{
			public const string DefaultSchemaName = "pf";

			public static class CoreSchema
			{
				public const string SchemaName = "core";

				public const string TenantTableName = "Tenants";
				public const string TenantRestrictedAssetTableName = "TenantRestrictedAssets";
				public const string TenantRestrictedPlatformTableName = "TenantRestrictedPlatforms";

				public const string PlatformTableName = "Platforms";
				public const string PlatformAccountTableName = "PlatformAccounts";
				public const string AccountTableName = "Accounts";

				public const string AssetTableName = "Assets";
			}

			public static class LedgerSchema
			{
				public const string SchemaName = "ledger";

				public const string ActivityTableName = "Activities";
				public const string ActivityLegTableName = "ActivityLegs";
			}

			public static class AuthSchema
			{
				public const string SchemaName = "auth";

				public const string ExternalIdentityTableName = "ExternalIdentities";
				public const string PasskeyCredentialsTableName = "PassKeyCredentials";
				public const string SessionTokenTableName = "SessionTokens";
			}
		}
	}
}