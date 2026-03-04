using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Flow.Rules;

internal class ActivityCommandGuards
{
	public static void EnsureMovementNotEmpty(Quantity fromAmount, Quantity toAmount)
	{
		if (fromAmount.IsZero && toAmount.IsZero)
		{
			throw new BadRequestException("At least one of FromAmount or ToAmount must be > 0.");
		}
	}

	public static void EnsureTradeOrExchangeShape(Quantity fromAmount, Quantity toAmount)
	{
		if (fromAmount.IsZero || toAmount.IsZero)
		{
			throw new BadRequestException("Both FromAmount and ToAmount must be > 0.");
		}
	}

	public static void EnsureFeeConsistency(Quantity? feeAmount, AssetId? feeAssetId)
	{
		if ((feeAssetId is null) != (feeAmount is null))
		{
			throw new BadRequestException("FeeAssetId and FeeAmount must be provided together.");
		}
	}
}
