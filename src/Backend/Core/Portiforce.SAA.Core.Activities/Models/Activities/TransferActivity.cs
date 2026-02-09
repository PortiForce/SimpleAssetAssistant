using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Activities.Models.Activities;

public sealed record TransferActivity : AssetActivityBase
{
	// not public, init only via factory
	private TransferActivity(
		ActivityId id,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		AssetActivityKind kind,
		DateTimeOffset occuredAt,
		ExternalMetadata externalMetadata,
		IReadOnlyList<AssetMovementLeg> legs,
		TransferKind transferKind,
		TransferDirection direction,
		string? reference,
		string? counterparty)
		: base(
			id,
			tenantId,
			platformAccountId,
			kind, occuredAt,
			externalMetadata,
			legs)
	{
		TransferKind = transferKind;
		Direction = direction;
		Reference = reference;
		Counterparty = counterparty;
	}

	// Private Empty Constructor for EF Core
	private TransferActivity() : base() { }

	public TransferKind TransferKind { get; init; }
	public TransferDirection Direction { get; init; }

	public string? Reference { get; init; }
	public string? Counterparty { get; init; }

	public static TransferActivity Create(
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		DateTimeOffset occurredAt,
		TransferKind transferKind,
		TransferDirection direction,
		IReadOnlyList<AssetMovementLeg> legs,
		string? reference,
		string? counterparty,
		ExternalMetadata externalMetadata,
		ActivityId? id)
	{
		LegGuards.EnforceCommonRules(legs);
		LegGuards.EnsureTransferShape(direction, legs);

		return new TransferActivity(
			id ?? ActivityId.New(),
			tenantId,
			platformAccountId,
			AssetActivityKind.Transfer,
			occurredAt,
			externalMetadata,
			legs,
			transferKind,
			direction,
			reference,
			counterparty);
	}
}