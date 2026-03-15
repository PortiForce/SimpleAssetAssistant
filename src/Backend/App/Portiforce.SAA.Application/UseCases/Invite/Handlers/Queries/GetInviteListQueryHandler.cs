using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteListQueryHandler(
	IClock clock,
	IInviteReadRepository inviteReadRepository
) : IRequestHandler<GetInviteListQuery, PagedResult<InviteListItem>>
{
	public async ValueTask<PagedResult<InviteListItem>> Handle(
		GetInviteListQuery request,
		CancellationToken ct)
	{
		var normalizedSearch = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();

		PagedResult<InviteListItemRaw> pagedInvitesRaw = await inviteReadRepository.GetListAsync(
			request.TenantId,
			request.Channels,
			request.States,
			normalizedSearch,
			request.HasAccount,
			request.PageRequest,
			ct);

		DateTimeOffset now = clock.UtcNow;

		List<InviteListItem> pagedInvites = pagedInvitesRaw.Items
			.Select(x => InviteProjectionMapper.ToListItem(x, now))
			.ToList();

		var result = new PagedResult<InviteListItem>(
			pagedInvites,
			pagedInvitesRaw.TotalCount,
			pagedInvitesRaw.PageNumber,
			pagedInvitesRaw.PageSize);

		return result;
	}
}
