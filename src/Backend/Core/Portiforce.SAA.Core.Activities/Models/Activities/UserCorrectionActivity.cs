using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Activities.Models.Activities;

public sealed record UserCorrectionActivity : ReasonedActivity
{
	// not public, init only via factory
	private UserCorrectionActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs,
		AssetActivityReason reason)
		: base(
			id,
			tenantId,
			platformAccountId,
			kind,
			occuredAt,
			externalMetadata,
			legs,
			reason)
	{
	}

	// Private Empty Constructor for EF Core
	private UserCorrectionActivity() : base() { }

	public static UserCorrectionActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata,
		ActivityId? id)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.UserCorrection, reason);

		LegGuards.EnforceCommonRules(legs);

		return new UserCorrectionActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			AssetActivityKind.UserCorrection,
			occurredAt,
			externalMetadata,
			legs,
			reason);
	}
}
