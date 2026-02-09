using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Futures;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Activities.Models.Activities;

public sealed record TradeActivity : ExecutableActivity
{
	// not public, init only via factory
	private TradeActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs,
		AssetActivityReason reason,
		CompletionType completionType,
		TradeExecutionType executionType,
		MarketKind marketKind,
		FuturesDescriptor? futures)
		: base(
			id,
			tenantId,
			platformAccountId,
			kind,
			occuredAt,
			externalMetadata,
			legs,
			reason,
			completionType)
	{
		ExecutionType = executionType;
		MarketKind = marketKind;
		Futures = futures;
	}

	// Private Empty Constructor for EF Core
	private TradeActivity() : base() { }

	public TradeExecutionType ExecutionType { get; init; }

	public MarketKind MarketKind { get; init; }

	/// <summary>
	/// Futures-aware minimal fields.
	/// </summary>
	public FuturesDescriptor? Futures { get; init; }

	public static TradeActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		MarketKind marketKind,
		TradeExecutionType executionType,
		IReadOnlyList<AssetMovementLeg> legs,
		FuturesDescriptor? futures,
		ExternalMetadata externalMetadata,
		CompletionType  completionType,
		ActivityId? id)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Trade, reason);

		LegGuards.EnforceCommonRules(legs);
		LegGuards.EnsureTradeOrExchangeShape(legs);

		if (marketKind == MarketKind.Futures)
		{
			LegGuards.EnsureFuturesAllocation(marketKind, legs);
			if (futures is null)
			{
				throw new DomainValidationException("Futures models should not be empty for Futures trade");
			}
		}
		else if (marketKind == MarketKind.Spot && futures is not null)
		{
			throw new DomainValidationException("Futures models should be null for Spot trade");

		}

		return new TradeActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			AssetActivityKind.Trade,
			occurredAt,
			externalMetadata,
			legs,
			reason,
			completionType,
			executionType,
			marketKind,
			futures);
	}
}
