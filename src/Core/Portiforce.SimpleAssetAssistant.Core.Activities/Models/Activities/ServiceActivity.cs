using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record ServiceActivity : ReasonedActivity
{
	// not public, init only via factory
	private ServiceActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs,
		AssetActivityReason reason,
		ServiceType serviceType)
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
		ServiceType = serviceType;
	}

	// Private Empty Constructor for EF Core
	private ServiceActivity() : base() { }

	public ServiceType ServiceType { get; init; }

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

		return new ServiceActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			AssetActivityKind.Service,
			occurredAt,
			externalMetadata,
			legs,
			reason,
			serviceType);
	}
}
