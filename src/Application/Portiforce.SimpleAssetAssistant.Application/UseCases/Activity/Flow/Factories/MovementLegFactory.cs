using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Factories;

internal static class MovementLegFactory
{
	public static List<AssetMovementLeg> CreateSpotTwoLegsWithOptionalFee(
		ActivityId activityId,
		AssetId outId,
		Quantity outAmount,
		byte outDecimals,
		AssetId inId,
		Quantity inAmount,
		byte inDecimals,
		AssetId? feeId,
		Quantity? feeAmount,
		byte? feeDecimals)
	{
		List<AssetMovementLeg> legs = new List<AssetMovementLeg>(capacity: feeId is null ? 2 : 3)
		{
			AssetMovementLeg.Create(
				activityId,
				outId,
				outAmount,
				MovementRole.Principal,
				MovementDirection.Outflow,
				AssetAllocationType.Spot,
				outDecimals),

			AssetMovementLeg.Create(
				activityId,
				inId,
				inAmount,
				MovementRole.Principal,
				MovementDirection.Inflow,
				AssetAllocationType.Spot,
				inDecimals),
		};

		if (feeId is not null && feeAmount is { IsPositive: true })
		{
			legs.Add(
				AssetMovementLeg.Create(
					activityId,
					feeId.Value,
					feeAmount,
					MovementRole.Fee,
					MovementDirection.Outflow,
					AssetAllocationType.Spot,
					feeDecimals!.Value));
		}

		return legs;
	}
}
