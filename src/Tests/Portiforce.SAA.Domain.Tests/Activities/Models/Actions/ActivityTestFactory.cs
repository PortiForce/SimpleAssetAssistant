using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

internal static class ActivityTestFactory
{
	public static ExternalMetadata ExternalMetadata()
		=> new(source: "tests", externalId: "ext-1");

	public static AssetMovementLeg PrincipalLeg(
		MovementDirection direction,
		AssetId? assetId = null,
		decimal amount = 1m,
		byte nativeDecimals = 8)
		=> AssetMovementLeg.Create(
			activityId: ActivityId.New(),
			assetId: assetId ?? AssetId.New(),
			amount: Quantity.Create(amount),
			role: MovementRole.Principal,
			direction: direction,
			allocation: AssetAllocationType.Spot,
			nativeDecimals: nativeDecimals);

	public static AssetMovementLeg FeeLegOutflow(
		AssetId? assetId = null,
		decimal amount = 0.1m,
		byte nativeDecimals = 8)
		=> AssetMovementLeg.Create(
			activityId: ActivityId.New(),
			assetId: assetId ?? AssetId.New(),
			amount: Quantity.Create(amount),
			role: MovementRole.Fee,
			direction: MovementDirection.Outflow,
			allocation: AssetAllocationType.Spot,
			nativeDecimals: nativeDecimals);
}