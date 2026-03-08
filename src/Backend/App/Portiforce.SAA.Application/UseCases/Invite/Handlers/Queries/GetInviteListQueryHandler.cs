using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteListQueryHandler(
	IInviteReadRepository inviteReadRepository
) : IRequestHandler<GetInviteListQuery, PagedResult<InviteListItem>>
{
	public async ValueTask<PagedResult<InviteListItem>> Handle(GetInviteListQuery request, CancellationToken ct)
	{
		PagedResult<InviteListItem> pagedInvites = await inviteReadRepository.GetListAsync(
			request.TenantId,
			request.Channel,
			request.State,
			request.Search,
			request.PageRequest,
			ct);

		return pagedInvites;
	}
}
