using System.ComponentModel.DataAnnotations;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
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
		string? instrumentKey = null)
	{
		if (amount == Quantity.Zero)
		{
			throw new ValidationException("Amount should be a positive value for a leg");
		}

		if (assetId.IsEmpty)
		{
			throw new ValidationException("AssetId should be defined value for a leg");
		}

		if (role == MovementRole.Fee && direction != MovementDirection.Outflow)
		{
			throw new ValidationException($"Fee leg always should be of outflow type, currentType: {direction}");
		}

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
