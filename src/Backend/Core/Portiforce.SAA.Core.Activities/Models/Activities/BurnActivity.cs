using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Activities.Models.Activities;

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
