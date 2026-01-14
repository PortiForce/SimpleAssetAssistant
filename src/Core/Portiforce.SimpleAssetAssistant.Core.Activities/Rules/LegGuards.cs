using System.ComponentModel.DataAnnotations;

using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Rules;

public static class LegGuards
{
	public static void EnsureNotNullOrEmpty(IReadOnlyList<AssetMovementLeg> legs)
	{
		if (legs is null || legs.Count == 0)
		{
			throw new ValidationException("Legs are required.");
		}
	}

	public static void EnsureFeeLegsAreValid(IReadOnlyList<AssetMovementLeg> legs)
	{
		foreach (var fee in legs.Where(l => l.Role == MovementRole.Fee))
		{
			if (fee.Direction != MovementDirection.Outflow)
			{
				throw new ValidationException("Fee legs must be Outflow.");
			}
		}
	}

	public static void EnsureTradeOrExchangeShape(IReadOnlyList<AssetMovementLeg> legs)
	{
		var principal = legs.Where(l => l.Role == MovementRole.Principal).ToList();
		if (principal.Count < 2)
		{
			throw new ValidationException("Trade/Exchange must contain at least 2 principal legs.");
		}

		if (principal.All(l => l.Direction != MovementDirection.Outflow) ||
		    principal.All(l => l.Direction != MovementDirection.Inflow))
		{
			throw new ValidationException("Trade/Exchange must contain both Outflow and Inflow principal legs.");
		}
			

		// optional: prevent same-asset principal swap (usually invalid)
		var outAssetIds = principal.Where(l => l.Direction == MovementDirection.Outflow).Select(l => l.AssetId).ToHashSet();
		var inAssetIds = principal.Where(l => l.Direction == MovementDirection.Inflow).Select(l => l.AssetId).ToHashSet();
		if (outAssetIds.Overlaps(inAssetIds))
		{
			throw new ValidationException("Trade/Exchange principal inflow and outflow assets must differ.");
		}
	}

	public static void EnsureTransferShape(TransferDirection direction, IReadOnlyList<AssetMovementLeg> legs)
	{
		if (direction == TransferDirection.Deposit)
		{
			EnsureOneSidedInflow(legs);
		}
		else
		{
			EnsureOneSidedOutflow(legs);
		}
	}

	public static void EnsureOneSidedInflow(IReadOnlyList<AssetMovementLeg> legs)
	{
		var principal = legs.Where(l => l.Role == MovementRole.Principal).ToList();
		if (principal.Count == 0 || principal.Any(l => l.Direction != MovementDirection.Inflow))
		{
			throw new ValidationException("This activity must have only Inflow principal legs.");
		}
	}

	public static void EnsureOneSidedOutflow(IReadOnlyList<AssetMovementLeg> legs)
	{
		var principal = legs.Where(l => l.Role == MovementRole.Principal).ToList();
		if (principal.Count == 0 || principal.Any(l => l.Direction != MovementDirection.Outflow))
		{
			throw new ValidationException("This activity must have only Outflow principal legs.");
		}
	}

	public static void EnsureFuturesAllocation(MarketKind marketKind, IReadOnlyList<AssetMovementLeg> legs)
	{
		if (marketKind == MarketKind.Futures)
		{
			// minimal invariant: at least one principal leg is Futures allocation
			if (!legs.Any(l => l is {Role: MovementRole.Principal, Allocation: AssetAllocationType.Futures}))
			{
				throw new ValidationException("Futures activities must include at least one Futures principal leg.");
			}
		}
	}
}

