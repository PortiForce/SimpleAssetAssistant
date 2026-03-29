namespace Portiforce.SAA.Core.Identity.Models.Client;

public sealed record TenantSettings
{
	private TenantSettings(
		TenantDefaults defaults,
		TenantSecuritySettings securitySettings,
		TenantImportSettings importSettings,
		TenantRetentionSettings retention)
	{
		this.Defaults = defaults;
		this.Security = securitySettings;
		this.Import = importSettings;
		this.Retention = retention;
	}

	private TenantSettings()
	{
		this.Defaults = TenantDefaults.Default();
		this.Security = new TenantSecuritySettings { EnforceTwoFactor = false };
		this.Retention = new TenantRetentionSettings { DeletedDataRetentionDays = 30 };
		this.Import = TenantImportSettings.Create(
			true,
			10_000,
			5);
	}

	public TenantDefaults Defaults { get; init; }

	public TenantSecuritySettings Security { get; init; }

	public TenantImportSettings Import { get; init; }

	public TenantRetentionSettings Retention { get; init; }

	public static TenantSettings Default() => new();
}