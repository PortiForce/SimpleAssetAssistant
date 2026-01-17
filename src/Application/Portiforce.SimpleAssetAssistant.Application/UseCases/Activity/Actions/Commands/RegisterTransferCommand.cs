using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterTransferCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	DateTimeOffset OccurredAt,
	string AssetCode,
	decimal Amount,
	string Direction,
	decimal? FeeAmount,
	string? Reference,
	string? Counterparty,
	string Source
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
