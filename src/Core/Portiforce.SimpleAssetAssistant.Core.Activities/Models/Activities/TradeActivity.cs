using System.ComponentModel.DataAnnotations;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Futures;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record TradeActivity : ExecutableActivity
{
	public override AssetActivityKind Kind => AssetActivityKind.Trade;

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
		MarketKind kind,
		TradeExecutionType executionType,
		IReadOnlyList<AssetMovementLeg> legs,
		FuturesDescriptor? futures,
		ExternalMetadata externalMetadata)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Trade, reason);
		ConsistencyRules.EnforceExternalMetadataRules(externalMetadata);

		LegGuards.EnsureNotNullOrEmpty(legs);
		LegGuards.EnsureFeeLegsAreValid(legs);
		LegGuards.EnsureTradeOrExchangeShape(legs);

		if (kind == MarketKind.Futures)
		{
			LegGuards.EnsureFuturesAllocation(kind, legs);
			if (futures is null)
			{
				throw new ValidationException("Futures models should not be empty for Futures trade");
			}
		}
		else if (kind == MarketKind.Spot && futures is not null)
		{
			throw new ValidationException("Futures models should be null for Spot trade");

		}

		return new TradeActivity
			{
				TenantId = tenantId,
				PlatformAccountId = platformAccountId,
				OccurredAt = occurredAt,
				MarketKind = kind,
				ExecutionType = executionType,
				Reason = reason,
				Legs = legs,
				ExternalMetadata = externalMetadata,
				Futures = futures
			};
	}
}
