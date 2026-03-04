using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;

public sealed record UpdateInviteCommand(
	TenantId TenantId,
	Guid Id,
	AccountTier Tier,
	Role Role,
	InviteState State
) : ICommand<TypedResult<Guid>>;
