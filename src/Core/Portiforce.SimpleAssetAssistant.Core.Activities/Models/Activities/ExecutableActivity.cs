using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record ExecutableActivity : ReasonedActivity
{
	protected ExecutableActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs,
		AssetActivityReason reason,
		CompletionType completionType) 
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
		CompletionType = completionType;
	}

	// Private Empty Constructor for EF Core
	protected ExecutableActivity(): base() { }

	public CompletionType CompletionType { get; init; }
}
