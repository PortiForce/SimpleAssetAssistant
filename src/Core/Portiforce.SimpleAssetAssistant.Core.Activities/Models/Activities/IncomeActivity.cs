using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record IncomeActivity(ActivityId Id) : ReasonedActivity(Id)
{
	public override AssetActivityKind Kind => AssetActivityKind.Income;

	public static IncomeActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata,
		ActivityId? id)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Income, reason);

		LegGuards.EnforceCommonRules(legs);
		LegGuards.EnsureOneSidedInflow(legs);

		return new IncomeActivity(id ?? ActivityId.New())
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
