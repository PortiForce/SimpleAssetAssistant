using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Messaging.Flow.Mappers;
using Portiforce.SAA.Application.UseCases.Messaging.Projections;

namespace Portiforce.SAA.Application.UseCases.Messaging.Handlers.Queries;

internal sealed class GetInboxMessageDetailsQueryHandler(IInboxMessageReadRepository inboxMessageReadRepository)
	: IRequestHandler<GetInboxMessageDetailsQuery, TypedResult<InboxMessageDetails>>
{
	public async ValueTask<TypedResult<InboxMessageDetails>> Handle(
		GetInboxMessageDetailsQuery request,
		CancellationToken ct)
	{
		InboxMessage? inboxMessage = await inboxMessageReadRepository.GetByIdAsync(
			request.TenantId,
			request.InboxMessageId,
			ct);

		if (inboxMessage is null)
		{
			return TypedResult<InboxMessageDetails>.Fail(
				ResultError.NotFound(
					"Inbox message",
					request.InboxMessageId));
		}

		return TypedResult<InboxMessageDetails>.Ok(InboxMessageProjectionMapper.ToDetails(inboxMessage));
	}
}
