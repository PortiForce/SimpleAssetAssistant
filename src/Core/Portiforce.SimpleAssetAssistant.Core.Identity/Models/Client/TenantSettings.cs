namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed record TenantSettings
{
	private TenantSettings(
		TenantDefaults defaults,
		TenantSecuritySettings securitySettings,
		TenantImportSettings importSettings,
		TenantRetentionSettings retention)
	{
		Defaults = defaults;
		Security = securitySettings;
		Import = importSettings;
		Retention = retention;
	}

	private TenantSettings()
	{
		Defaults = TenantDefaults.Default();
		Security = new TenantSecuritySettings { EnforceTwoFactor = false };
		Retention = new TenantRetentionSettings { DeletedDataRetentionDays = 30 };
		Import = TenantImportSettings.Create(
			requireReviewBeforeProcessing: true,
			maxRowsPerImport: 10_000,
			maxFileSizeMb: 5);
	}

	public TenantDefaults Defaults { get; init; }
	public TenantSecuritySettings Security { get; init; }
	public TenantImportSettings Import { get; init; }
	public TenantRetentionSettings Retention { get; init; }

	public static TenantSettings Default()
	{
		return new TenantSettings();
	}
}
