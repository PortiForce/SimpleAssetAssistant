using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;

/// <summary>
/// Invite user flow
/// </summary>
/// <param name="TenantId"></param>
/// <param name="Email"></param>
/// <param name="Alias"></param>
/// <param name="Role"></param>
/// <param name="Tier"></param>
public sealed record CreateAccountCommand(
	TenantId TenantId,
	Email Email,
	string Alias,
	Role Role,
	AccountTier Tier
) : ICommand<CommandResult<AccountId>>;
