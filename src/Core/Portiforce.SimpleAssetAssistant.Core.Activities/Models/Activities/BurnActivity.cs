using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record BurnActivity: ReasonedActivity
{
	// not public, init only via factory
	private BurnActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata)
		: base(
			id,
			tenantId,
			platformAccountId,
			AssetActivityKind.Burn,
			occurredAt,
			externalMetadata,
			legs,
			reason)
	{
		
	}

	// Private Empty Constructor for EF Core
	private BurnActivity() : base() { }

	public static BurnActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata,
		ActivityId? id)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Burn, reason);

		LegGuards.EnforceCommonRules(legs);
		LegGuards.EnsureOneSidedOutflow(legs);

		return new BurnActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			occurredAt,
			reason,
			legs,
			externalMetadata);
	}
}
