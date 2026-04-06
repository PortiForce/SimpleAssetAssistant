using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;

/// <summary>
///     Represents a command to create an invite for a specified target within a tenant, assigning an intended role and
///     account tier.
/// </summary>
/// <param name="TenantId">The identifier of the tenant in which the invite is being created.</param>
/// <param name="InviteTarget">The target entity to be invited, such as a user or email address.</param>
/// <param name="IntendedRole">The role that will be assigned to the invited entity upon acceptance.</param>
/// <param name="IntendedTier">The account tier that will be assigned to the invited entity upon acceptance.</param>
/// <param name="InvitedByAccountId">The identifier of the account that is creating the invite.</param>
/// <param name="Alias">An optional alias or display name associated with the invite.</param>
/// <param name="CreatedAtUtc">The date and time, in UTC, when the invite was created.</param>
/// <param name="ExpiredAtUtc">The date and time, in UTC, when the invite will expire.</param>
public sealed record CreateInviteCommand(
	TenantId TenantId,
	InviteTarget InviteTarget,
	Role IntendedRole,
	AccountTier IntendedTier,
	AccountId InvitedByAccountId,
	string Alias,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiredAtUtc) : ICommand<TypedResult<CreateInviteResult>>;