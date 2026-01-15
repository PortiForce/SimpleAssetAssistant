using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record AssetActivityBase: IAggregateRoot
{
	public abstract AssetActivityKind Kind { get; }

	public ActivityId Id { get; init; } = ActivityId.New();

	public required TenantId TenantId { get; init; }
	public required PlatformAccountId PlatformAccountId { get; init; }

	public required DateTimeOffset OccurredAt { get; init; }

	public required ExternalMetadata ExternalMetadata { get; init; }

	public required IReadOnlyList<AssetMovementLeg> Legs { get; init; }
}