using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;

public sealed record DeclineInviteCommand(
	TenantId TenantId,
	string RawToken) : ICommand<TypedResult<DeclineInviteResult>>;