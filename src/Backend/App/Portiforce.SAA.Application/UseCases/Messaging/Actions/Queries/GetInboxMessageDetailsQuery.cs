using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Messaging.Actions.Queries;

public sealed record GetInboxMessageDetailsQuery(
	TenantId TenantId,
	Guid InboxMessageId) : IQuery<TypedResult<InboxMessageDetails>>;
