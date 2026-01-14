using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public sealed record TransferActivity : AssetActivityBase
{
	public override AssetActivityKind Kind => AssetActivityKind.Transfer;

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
		ExternalMetadata externalMetadata)
	{
		ConsistencyRules.EnforceExternalMetadataRules(externalMetadata);

		LegGuards.EnsureNotNullOrEmpty(legs);
		LegGuards.EnsureFeeLegsAreValid(legs);
		LegGuards.EnsureTransferShape(direction, legs);
		
		return new TransferActivity
		{
			TenantId = tenantId,
			PlatformAccountId = platformAccountId,
			OccurredAt = occurredAt,
			TransferKind = transferKind,
			Direction = direction,
			Reference = reference,
			Counterparty = counterparty,
			Legs = legs,
			ExternalMetadata = externalMetadata
		};
	}
}