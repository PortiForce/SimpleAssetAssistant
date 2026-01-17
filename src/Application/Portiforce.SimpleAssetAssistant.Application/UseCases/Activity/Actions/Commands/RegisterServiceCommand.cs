using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterServiceCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	DateTimeOffset OccurredAt,
	string ServiceType,
	string AssetCode,
	decimal Amount,
	string FeeReason,
	string Source,
	string? ExternalId
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
