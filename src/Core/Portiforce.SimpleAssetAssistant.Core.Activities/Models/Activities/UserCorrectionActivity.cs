using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record UserActivity : ReasonedActivity
{
	public override AssetActivityKind Kind => AssetActivityKind.UserCorrection;

	public static BurnActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.UserCorrection, reason);
		ConsistencyRules.EnforceExternalMetadataRules(externalMetadata);

		LegGuards.EnsureNotNullOrEmpty(legs);
		LegGuards.EnsureFeeLegsAreValid(legs);
		LegGuards.EnsureOneSidedOutflow(legs);

		return new BurnActivity
		{
			TenantId = tenantId,
			PlatformAccountId = platformAccountId,
			OccurredAt = occurredAt,
			Reason = reason,
			Legs = legs,
			ExternalMetadata = externalMetadata
		};
	}
}
