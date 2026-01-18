using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Actions.Commands;

public sealed record AddPlatformAccountCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	string AccountName,
	string? ExternalUserId
) : ICommand<BaseCreateCommandResponse<PlatformAccountId>>;
