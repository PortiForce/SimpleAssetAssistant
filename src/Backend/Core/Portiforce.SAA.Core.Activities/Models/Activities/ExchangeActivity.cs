using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Activities.Models.Activities;

public sealed record ExchangeActivity : ExecutableActivity
{
	// not public, init only via factory
	private ExchangeActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata, 
		CompletionType completionType,
		ExchangeType exchangeType)
		: base(
			id,
			tenantId,
			platformAccountId,
			AssetActivityKind.Exchange,
			occurredAt,
			externalMetadata,
			legs,
			reason,
			completionType)
	{
		ExchangeType = exchangeType;
	}

	// Private Empty Constructor for EF Core
	private ExchangeActivity(): base() { }

	public ExchangeType ExchangeType { get; init; }

	public static ExchangeActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		ExchangeType exchangeType,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata,
		CompletionType completionType,
		ActivityId? id)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Exchange, reason);

		LegGuards.EnforceCommonRules(legs);
		LegGuards.EnsureTradeOrExchangeShape(legs);

		return new ExchangeActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			occurredAt,
			reason,
			legs,
			externalMetadata,
			completionType,
			exchangeType);
	}
}