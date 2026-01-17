using Portiforce.SimpleAssetAssistant.Core.Activities.Models;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Rules;

public static class ConsistencyRules
{
	public static void EnsureMovementNotEmpty(Quantity fromAmount, Quantity toAmount)
	{
		if (fromAmount.Value == 0m && toAmount.Value == 0m)
		{
			throw new DomainValidationException("At least one of FromAmount or ToAmount must be > 0.");
		}
	}

	public static void EnsureFeeConsistency(Quantity feeAmount, AssetId? feeAssetId)
	{
		if (feeAmount.Value > 0 && feeAssetId is null)
		{
			throw new DomainValidationException("FeeAssetId is required when FeeAmount > 0.");
		}

		if (feeAmount.Value == 0 && feeAssetId is not null)
		{
			throw new DomainValidationException("FeeAssetId must be null when FeeAmount is zero.");
		}
	}
}
