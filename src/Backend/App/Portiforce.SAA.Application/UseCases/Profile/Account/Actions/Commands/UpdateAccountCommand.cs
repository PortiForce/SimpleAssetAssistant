using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;

public sealed record UpdateAccountCommand(
	TenantId TenantId,
	AccountId Id,
	string Alias,
	AccountTier Tier,
	Role Role,
	AccountState State
) : ICommand<CommandResult<AccountId>>;
