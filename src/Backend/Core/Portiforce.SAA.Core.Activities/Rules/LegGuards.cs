using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Core.Activities.Rules;

public static class LegGuards
{
	public static void EnforceCommonRules(IReadOnlyList<AssetMovementLeg> legs)
	{
		EnsureNotNullOrEmpty(legs);
		EnsureFeeLegsAreValid(legs);
		EnsureAllLegAmountsPositive(legs);
	}

	public static void EnsureNotNullOrEmpty(IReadOnlyList<AssetMovementLeg> legs)
	{
		if (legs is null || legs.Count == 0)
		{
			throw new DomainValidationException("Legs are required.");
		}
	}

	public static void EnsureFeeLegsAreValid(IReadOnlyList<AssetMovementLeg> legs)
	{
		foreach (var fee in legs.Where(l => l.Role == MovementRole.Fee))
		{
			if (fee.Direction != MovementDirection.Outflow)
			{
				throw new DomainValidationException("Fee legs must be Outflow.");
			}
		}
	}

	public static void EnsureAllLegAmountsPositive(IReadOnlyList<AssetMovementLeg> legs)
	{
		if (legs.Any(l => l.Amount.Value <= 0m))
		{
			throw new DomainValidationException("All leg amounts must be > 0.");
		}
	}

	public static void EnsureTradeOrExchangeShape(IReadOnlyList<AssetMovementLeg> legs)
	{
		var principal = legs.Where(l => l.Role == MovementRole.Principal).ToList();
		if (principal.Count < 2)
		{
			throw new DomainValidationException("Trade/Exchange must contain at least 2 principal legs.");
		}

		if (principal.All(l => l.Direction != MovementDirection.Outflow) ||
		    principal.All(l => l.Direction != MovementDirection.Inflow))
		{
			throw new DomainValidationException("Trade/Exchange must contain both Outflow and Inflow principal legs.");
		}
			

		// optional: prevent same-asset principal swap (usually invalid)
		var outAssetIds = principal.Where(l => l.Direction == MovementDirection.Outflow).Select(l => l.AssetId).ToHashSet();
		var inAssetIds = principal.Where(l => l.Direction == MovementDirection.Inflow).Select(l => l.AssetId).ToHashSet();
		if (outAssetIds.Overlaps(inAssetIds))
		{
			throw new DomainValidationException("Trade/Exchange principal inflow and outflow assets must differ.");
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
			throw new DomainValidationException("This activity must have only Inflow principal legs.");
		}
	}

	public static void EnsureOneSidedOutflow(IReadOnlyList<AssetMovementLeg> legs)
	{
		var principal = legs.Where(l => l.Role == MovementRole.Principal).ToList();
		if (principal.Count == 0 || principal.Any(l => l.Direction != MovementDirection.Outflow))
		{
			throw new DomainValidationException("This activity must have only Outflow principal legs.");
		}
	}

	public static void EnsureFuturesAllocation(MarketKind marketKind, IReadOnlyList<AssetMovementLeg> legs)
	{
		if (marketKind == MarketKind.Futures)
		{
			// minimal invariant: at least one principal leg is Futures allocation
			if (!legs.Any(l => l is {Role: MovementRole.Principal, Allocation: AssetAllocationType.Futures}))
			{
				throw new DomainValidationException("Futures activities must include at least one Futures principal leg.");
			}
		}
		else
		{
			throw new NotSupportedException(
				$"{marketKind} is not intended to be used for {nameof(EnsureFuturesAllocation)}");
		}
	}
}

