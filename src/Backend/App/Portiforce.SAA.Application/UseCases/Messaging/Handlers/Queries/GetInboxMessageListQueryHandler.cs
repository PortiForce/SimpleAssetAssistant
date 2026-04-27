using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Messaging.Flow.Mappers;
using Portiforce.SAA.Application.UseCases.Messaging.Projections;

namespace Portiforce.SAA.Application.UseCases.Messaging.Handlers.Queries;

internal sealed class GetInboxMessageListQueryHandler(IInboxMessageReadRepository inboxMessageReadRepository)
	: IRequestHandler<GetInboxMessageListQuery, PagedResult<InboxMessageListItem>>
{
	public async ValueTask<PagedResult<InboxMessageListItem>> Handle(
		GetInboxMessageListQuery request,
		CancellationToken ct)
	{
		string? normalizedSearch = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();

		PagedResult<InboxMessage> pagedInboxMessages = await inboxMessageReadRepository.GetListAsync(
			request.TenantId,
			request.States,
			normalizedSearch,
			request.PageRequest,
			ct);

		List<InboxMessageListItem> items = pagedInboxMessages.Items
			.Select(InboxMessageProjectionMapper.ToListItem)
			.ToList();

		return new PagedResult<InboxMessageListItem>(
			items,
			pagedInboxMessages.TotalCount,
			pagedInboxMessages.PageNumber,
			pagedInboxMessages.PageSize);
	}
}
