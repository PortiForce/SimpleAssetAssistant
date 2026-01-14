namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed record TenantSettings
{
	public required TenantDefaults Defaults { get; init; }
	public required TenantSecuritySettings Security { get; init; }
	public required TenantImportSettings Import { get; init; }
	public required TenantRetentionSettings Retention { get; init; }

	public static TenantSettings Default() => new()
	{
		Defaults = TenantDefaults.Default(),
		Security = new TenantSecuritySettings { EnforceTwoFactor = false },
		Retention = new TenantRetentionSettings { DeletedDataRetentionDays = 30 },
		Import = TenantImportSettings.Create(
			requireReviewBeforeProcessing: true,
			maxRowsPerImport: 10_000,
			maxFileSizeMb: 5),
	};
}
