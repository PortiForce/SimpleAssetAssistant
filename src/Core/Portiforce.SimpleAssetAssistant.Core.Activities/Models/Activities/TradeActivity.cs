using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Futures;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record TradeActivity(ActivityId Id) : ExecutableActivity(Id)
{
	public override AssetActivityKind Kind { get; init; } = AssetActivityKind.Trade;

	public TradeExecutionType ExecutionType { get; init; } = TradeExecutionType.NotDefined;

	public MarketKind MarketKind { get; init; } = MarketKind.Spot;

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

		return new TradeActivity(id ?? ActivityId.New())
		{
			TenantId = tenantId,
			PlatformAccountId = platformAccountId,
			OccurredAt = occurredAt,
			MarketKind = marketKind,
			ExecutionType = executionType,
			Reason = reason,
			Legs = legs,
			ExternalMetadata = externalMetadata,
			Futures = futures
		};
	}
}
