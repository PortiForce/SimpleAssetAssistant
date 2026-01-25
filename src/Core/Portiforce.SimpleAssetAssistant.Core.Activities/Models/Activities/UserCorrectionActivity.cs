using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

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
