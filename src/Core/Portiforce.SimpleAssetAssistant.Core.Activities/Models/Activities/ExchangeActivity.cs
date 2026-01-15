using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record ExchangeActivity(ActivityId Id) : ExecutableActivity(Id)
{
	public override AssetActivityKind Kind => AssetActivityKind.Exchange;

	public ExchangeType ExchangeType { get; init; } = ExchangeType.Direct;

	public static ExchangeActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		ExchangeType exchangeType,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata,
		ActivityId? id)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Exchange, reason);
		ConsistencyRules.EnforceExternalMetadataRules(externalMetadata);

		LegGuards.EnsureNotNullOrEmpty(legs);
		LegGuards.EnsureFeeLegsAreValid(legs);
		LegGuards.EnsureTradeOrExchangeShape(legs);

		return new ExchangeActivity(id ?? ActivityId.New())
		{
			TenantId = tenantId,
			PlatformAccountId = platformAccountId,
			OccurredAt = occurredAt,
			Reason = reason,
			ExchangeType = exchangeType,
			Legs = legs,
			ExternalMetadata = externalMetadata,
		};
	}
}