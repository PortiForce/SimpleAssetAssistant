using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Commands;

public sealed record UpdateAccountCommand(
	TenantId TenantId,
	AccountId Id,
	string Alias,
	AccountTier Tier,
	Role Role,
	AccountState State
) : ICommand<CommandResult<AccountId>>;
