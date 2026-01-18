using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;

public sealed record AssetMovementLeg
{
	public required AssetId AssetId { get; init; }

	public required Quantity Amount { get; init; }

	public required MovementDirection Direction { get; init; }

	public MovementRole Role { get; init; } = MovementRole.Principal;

	public AssetAllocationType Allocation { get; init; } = AssetAllocationType.Spot;

	public string? InstrumentKey { get; init; }

	public static AssetMovementLeg Create(
		AssetId assetId,
		Quantity amount,
		MovementRole role,
		MovementDirection direction,
		AssetAllocationType allocation,
		byte nativeDecimals,
		string? instrumentKey = null)
	{
		if (amount == Quantity.Zero)
		{
			throw new DomainValidationException("Amount should be a positive value for a leg");
		}

		if (assetId.IsEmpty)
		{
			throw new DomainValidationException("AssetId should be defined value for a leg");
		}

		if (role == MovementRole.Fee && direction != MovementDirection.Outflow)
		{
			throw new DomainValidationException($"Fee leg always should be of outflow type, currentType: {direction}");
		}

		ConsistencyRules.EnsureScaleDoesNotExceed(amount.Value, nativeDecimals, nameof(amount));

		var leg = new AssetMovementLeg
		{
			AssetId = assetId,
			Amount = amount,
			Direction = direction,
			Role = role,
			Allocation = allocation,
			InstrumentKey = instrumentKey
		};
		
		return leg;
	}
}
