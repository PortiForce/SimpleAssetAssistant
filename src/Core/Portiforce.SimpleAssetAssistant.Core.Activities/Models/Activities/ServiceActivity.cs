using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record ServiceActivity : ReasonedActivity
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
		ServiceType serviceType = ServiceType.Custody)
	{
		ActivityGuards.EnsureReasonKindPairAllowed(AssetActivityKind.Service, reason);
		ConsistencyRules.EnforceExternalMetadataRules(externalMetadata);

		LegGuards.EnsureNotNullOrEmpty(legs);
		LegGuards.EnsureFeeLegsAreValid(legs);
		LegGuards.EnsureOneSidedOutflow(legs);

		return new ServiceActivity
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
