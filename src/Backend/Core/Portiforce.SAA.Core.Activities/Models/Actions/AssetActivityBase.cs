using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Activities.Models.Actions;

public abstract record AssetActivityBase : Fact<ActivityId>, IAggregateRoot
{
	protected AssetActivityBase(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs)
		: base(id)
	{
		this.TenantId = tenantId;
		this.PlatformAccountId = platformAccountId;
		this.Kind = kind;
		this.OccurredAt = occuredAt;
		this.ExternalMetadata = externalMetadata;
		this.Legs = legs;
	}

	// Private Empty Constructor for EF Core
	protected AssetActivityBase()
	{
		this.Legs = [];
	}

	public TenantId TenantId { get; private set; }

	public PlatformAccountId PlatformAccountId { get; private set; }

	public AssetActivityKind Kind { get; private set; }

	public DateTimeOffset OccurredAt { get; private set; }

	public ExternalMetadata ExternalMetadata { get; private set; }

	public IReadOnlyList<AssetMovementLeg> Legs { get; private set; }
}