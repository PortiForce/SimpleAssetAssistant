using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;

public sealed record AssetMovementLeg : Fact<LegId>
{
	public AssetId AssetId { get; init; }

	public ActivityId ActivityId { get; init; }

	public Quantity Amount { get; init; }

	public MovementDirection Direction { get; init; }

	public MovementRole Role { get; init; } = MovementRole.Principal;

	public AssetAllocationType Allocation { get; init; } = AssetAllocationType.Spot;

	public string? InstrumentKey { get; init; }

	private AssetMovementLeg(
		LegId id,
		ActivityId activityId,
		AssetId assetId,
		Quantity amount,
		MovementRole role,
		MovementDirection direction,
		AssetAllocationType allocation,
		byte nativeDecimals,
		string? instrumentKey): base(id)
	{
		if (amount == Quantity.Zero)
		{
			throw new DomainValidationException("Amount should be a positive value for a leg");
		}

		if (assetId.IsEmpty)
		{
			throw new DomainValidationException("AssetId should be defined value for a leg");
		}

		if (activityId.IsEmpty)
		{
			throw new DomainValidationException("ActivityId should be defined value for a leg");
		}

		if (role == MovementRole.Fee && direction != MovementDirection.Outflow)
		{
			throw new DomainValidationException($"Fee leg always should be of outflow type, currentType: {direction}");
		}

		ConsistencyRules.EnsureScaleDoesNotExceed(amount.Value, nativeDecimals, nameof(amount));
		
		ActivityId = activityId;
		AssetId = assetId;
		Amount = amount;
		Role = role;
		Allocation = allocation;
		Direction = direction;
		InstrumentKey = instrumentKey;
	}

	public static AssetMovementLeg Create(
		ActivityId activityId,
		AssetId assetId,
		Quantity amount,
		MovementRole role,
		MovementDirection direction,
		AssetAllocationType allocation,
		byte nativeDecimals,
		LegId id = default,
		string? instrumentKey = null)
	=> new (
			id.IsEmpty ? LegId.New() : id,
			activityId,
			assetId,
			amount,
			role,
			direction,
			allocation,
			nativeDecimals,
			instrumentKey);
}
