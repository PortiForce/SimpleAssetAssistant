using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterIncomeCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	DateTimeOffset OccurredAt,

	string AssetCode,
	decimal Amount,
	string IncomeReason,      // "Staking", "Dividend", "Airdrop"

	string Source,
	string? ExternalId
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
