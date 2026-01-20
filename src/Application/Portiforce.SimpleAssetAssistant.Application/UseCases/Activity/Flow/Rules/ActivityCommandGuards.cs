using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Rules;

internal class ActivityCommandGuards
{
	public static void EnsureMovementNotEmpty(Quantity fromAmount, Quantity toAmount)
	{
		if (fromAmount.Value == 0m && toAmount.Value == 0m)
		{
			throw new BadRequestException("At least one of FromAmount or ToAmount must be > 0.");
		}
	}

	public static void EnsureFeeConsistency(Quantity? feeAmount, AssetId? feeAssetId)
	{
		if (feeAmount is { IsPositive: true } && feeAssetId is null)
		{
			throw new BadRequestException("FeeAssetId is required when FeeAmount > 0.");
		}

		if (feeAmount == null && feeAssetId is not null)
		{
			throw new BadRequestException("FeeAssetId must be null when FeeAmount is zero.");
		}
	}
}
