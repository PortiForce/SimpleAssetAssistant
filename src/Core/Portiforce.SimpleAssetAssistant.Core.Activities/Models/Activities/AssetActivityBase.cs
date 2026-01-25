using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record AssetActivityBase : Fact<ActivityId>, IAggregateRoot
{
	protected AssetActivityBase(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs
		) : base(id)
	{
		TenantId = tenantId;
		PlatformAccountId = platformAccountId;
		Kind = kind;
		OccurredAt = occuredAt;
		ExternalMetadata = externalMetadata;
		Legs = legs;
	}

	// Private Empty Constructor for EF Core
	protected AssetActivityBase()
	{
		Legs = new List<AssetMovementLeg>();
	}

	public TenantId TenantId { get; private set; }

	public PlatformAccountId PlatformAccountId { get; private set; }

	public AssetActivityKind Kind { get; private set; }

	public DateTimeOffset OccurredAt { get; private set; }

	public ExternalMetadata ExternalMetadata { get; private set; }

	public IReadOnlyList<AssetMovementLeg> Legs { get; private set; }
}