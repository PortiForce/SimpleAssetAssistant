using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Commands;

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
