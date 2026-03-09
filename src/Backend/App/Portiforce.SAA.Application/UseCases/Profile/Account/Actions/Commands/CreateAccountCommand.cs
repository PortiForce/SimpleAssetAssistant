using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Result;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;

/// <summary>
/// Create account flow
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
) : ICommand<TypedResult<CreateAccountResult>>;
