using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record IncomeActivity : ReasonedActivity
{
	// not public, init only via factory
	private IncomeActivity(
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
	private IncomeActivity() : base() { }

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

		return new IncomeActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			AssetActivityKind.Income,
			occurredAt,
			externalMetadata,
			legs,
			reason);
	}
}
