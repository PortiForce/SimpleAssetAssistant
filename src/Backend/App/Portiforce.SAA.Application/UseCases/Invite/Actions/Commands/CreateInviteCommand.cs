using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;

/// <summary>
/// Invite user flow
/// </summary>
/// <param name="TenantId"></param>
/// <param name="InviteTarget"></param>
/// <param name="IntendedRole"></param>
/// <param name="IntendedTier"></param>
/// <param name="InvitedByAccountId"></param>
/// <param name="CreatedAtUtc"></param>
/// <param name="ExpiredAtUtc"></param>
public sealed record CreateInviteCommand(
	TenantId TenantId,
	InviteTarget InviteTarget,
	Role IntendedRole,
	AccountTier IntendedTier,
	AccountId InvitedByAccountId,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiredAtUtc
) : ICommand<TypedResult<CreateInviteResult>>;
