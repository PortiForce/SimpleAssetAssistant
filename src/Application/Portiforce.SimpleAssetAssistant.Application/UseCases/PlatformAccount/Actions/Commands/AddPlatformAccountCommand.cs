using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Actions.Commands;

public sealed record AddPlatformAccountCommand(
	TenantId TenantId,
	AccountId AccountId,      // Owner
	PlatformId PlatformId,
	string AccountName,       // User's custom name: "My Main Binance"
	string? ExternalUserId    // API Key ID or similar reference
) : ICommand<BaseCreateCommandResponse<PlatformAccountId>>;
