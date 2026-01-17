using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed record TenantImportSettings
{
	private TenantImportSettings(
		bool requireReviewBeforeProcessing,
		int maxRowsPerImport,
		int maxFileSizeMb)
	{
		if (maxRowsPerImport > LimitationRules.Lengths.Tenant.MaxRowsPerFile)
		{
			throw new ArgumentOutOfRangeException(
				nameof(maxRowsPerImport),
				$"Max rows per import cannot exceed {LimitationRules.Lengths.Tenant.MaxRowsPerFile}.");
		}

		if (maxFileSizeMb > LimitationRules.Lengths.Tenant.MaxFileSizeMb)
		{
			throw new DomainValidationException(
				$"Max file size for upload is limited to {LimitationRules.Lengths.Tenant.MaxFileSizeMb} mb");
		}

		RequireReviewBeforeProcessing = requireReviewBeforeProcessing;
		MaxRowsPerImport = maxRowsPerImport;
		MaxFileSizeMb = maxFileSizeMb;
	}
	
	public bool RequireReviewBeforeProcessing { get; } = true;
	public int MaxRowsPerImport { get; }
	public int MaxFileSizeMb { get; }

	public static TenantImportSettings Create(
		bool requireReviewBeforeProcessing,
		int maxRowsPerImport,
		int maxFileSizeMb)
	{
		return new TenantImportSettings(requireReviewBeforeProcessing, maxRowsPerImport, maxFileSizeMb);
	}
}
