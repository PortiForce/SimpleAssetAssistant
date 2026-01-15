using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Commands;

public sealed record CreateAccountCommand(
	TenantId TenantId,
	string Email,
	string Alias,
	string Role               // "TenantUser", "TenantAdmin"
) : ICommand<BaseCreateCommandResponse<AccountId>>;
