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
	string Direction,         // "Deposit" or "Withdrawal"

	// Fee (Optional - usually withdrawal fee)
	string? FeeAssetCode,
	decimal? FeeAmount,

	string? Reference,        // e.g. TX Hash
	string? Counterparty,     // e.g. Wallet Address
	string Source
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
