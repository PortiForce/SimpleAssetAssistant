using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterBurnCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	DateTimeOffset OccurredAt,
	string AssetCode,
	decimal Amount,
	string BurnReason,
	string? Notes,
	string Source,
	string? ExternalId
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
