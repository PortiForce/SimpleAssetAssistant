using System.ComponentModel.DataAnnotations;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Rules;

public static class ConsistencyRules
{
	public static void EnforceExternalMetadataRules(ExternalMetadata externalMetadata)
	{
		EnsureSource(externalMetadata.Source);
		EnsureExternalRecordData(externalMetadata.ExternalId, externalMetadata.Fingerprint);
	}

	private static void EnsureSource(string? source)
	{
		if (string.IsNullOrWhiteSpace(source))
		{
			throw new ValidationException("Source is required.");
		}
	}

	private static void EnsureExternalRecordData(string? externalId, string? fingerprint)
	{
		if (string.IsNullOrWhiteSpace(externalId) && string.IsNullOrWhiteSpace(fingerprint))
		{
			throw new ValidationException("Either externalId or fingerprint are required.");
		}
	}

	public static void EnsureMovementNotEmpty(Quantity fromAmount, Quantity toAmount)
	{
		if (fromAmount.Value == 0m && toAmount.Value == 0m)
		{
			throw new ValidationException("At least one of FromAmount or ToAmount must be > 0.");
		}
	}

	public static void EnsureFeeConsistency(Quantity feeAmount, AssetId? feeAssetId)
	{
		if (feeAmount.Value > 0 && feeAssetId is null)
		{
			throw new ValidationException("FeeAssetId is required when FeeAmount > 0.");
		}

		if (feeAmount.Value == 0 && feeAssetId is not null)
		{
			throw new ValidationException("FeeAssetId must be null when FeeAmount is zero.");
		}
	}
}
