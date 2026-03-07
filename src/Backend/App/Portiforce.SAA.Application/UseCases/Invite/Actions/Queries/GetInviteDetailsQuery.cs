using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;

public sealed record GetInviteDetailsQuery(TenantId TenantId, Guid InviteId) : IQuery<TypedResult<InviteDetails>>;