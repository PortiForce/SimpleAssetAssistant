using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterCorrectionCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	AssetId AssetId,
	QuantityDelta DeltaAmount, // Not Quantity, as this might accept negative values as a correction element
	AssetActivityReason Reason,
	string? Notes
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
