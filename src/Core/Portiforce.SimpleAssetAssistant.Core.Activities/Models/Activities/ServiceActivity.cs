using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record ServiceActivity(ActivityId Id) : ReasonedActivity(Id)
{
	public override AssetActivityKind Kind => AssetActivityKind.Service;

	public required ServiceType ServiceType { get; init; }

	public static ServiceActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		AssetActivityReason reason,
		IReadOnlyList<AssetMovementLeg> legs,
		ExternalMetadata externalMetadata,
		ServiceType serviceType = ServiceType.Custody,
		ActivityId? id = null)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Service, reason);

		LegGuards.EnforceCommonRules(legs);
		LegGuards.EnsureOneSidedOutflow(legs);

		return new ServiceActivity(id ?? ActivityId.New())
		{
			TenantId = tenantId,
			PlatformAccountId = platformAccountId,
			OccurredAt = occurredAt,
			ServiceType = serviceType,
			Reason = reason,
			Legs = legs,
			ExternalMetadata = externalMetadata,
		};
	}
}
