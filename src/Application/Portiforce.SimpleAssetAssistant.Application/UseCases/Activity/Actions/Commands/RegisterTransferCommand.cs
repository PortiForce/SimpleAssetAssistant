using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterTransferCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	AssetId AssetId,
	Quantity Amount,
	MovementDirection Direction,
	Quantity? FeeAmount,
	string? Reference,
	string? Counterparty,
	string Source
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
