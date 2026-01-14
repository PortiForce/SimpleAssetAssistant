using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record IncomeActivity : ReasonedActivity
{
	public override AssetActivityKind Kind => AssetActivityKind.Income;

	public static IncomeActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Income, reason);
		ConsistencyRules.EnforceExternalMetadataRules(externalMetadata);

		LegGuards.EnsureNotNullOrEmpty(legs);
		LegGuards.EnsureFeeLegsAreValid(legs);
		LegGuards.EnsureOneSidedInflow(legs);

		return new IncomeActivity
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
