using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Assets.Extensions;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Rules;

internal static class ActivityReasonRules
{
	public static AssetActivityReason DetermineFromKinds(AssetKind outKind, AssetKind inKind)
	{
		bool isOutAssetFiatOrStableRelated = AssetExtensions.IsFiatOrStableKind(outKind);
		bool isInAssetFiatOrStableRelated = AssetExtensions.IsFiatOrStableKind(inKind);

		AssetActivityReason actualReason =
			isOutAssetFiatOrStableRelated && !isInAssetFiatOrStableRelated ? AssetActivityReason.Buy :
			!isOutAssetFiatOrStableRelated && isInAssetFiatOrStableRelated ? AssetActivityReason.Sell :
			AssetActivityReason.Conversion;

		return actualReason;
	}

	public static void EnsureIsTradeReason(AssetActivityReason reason, string primaryId)
	{
		if (reason == AssetActivityReason.Conversion)
		{
			throw new BadRequestException($"Trade handler is not suitable for reason: {reason}, id: {primaryId}");
		}
	}

	public static void EnsureIsExchangeReason(AssetActivityReason reason, string primaryId)
	{
		if (reason != AssetActivityReason.Conversion)
		{
			throw new BadRequestException($"Exchange handler is not suitable for reason: {reason}, id: {primaryId}");
		}
	}
}
