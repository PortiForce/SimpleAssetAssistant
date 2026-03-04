using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Handlers.Queries;

internal sealed class GetAccountListQueryHandler(
	IAccountReadRepository accountReadRepository)
	: IRequestHandler<GetAccountListQuery, PagedResult<AccountListItem>>
{
	public async ValueTask<PagedResult<AccountListItem>> Handle(
		GetAccountListQuery request,
		CancellationToken ct)
	{
		return await accountReadRepository.GetByTenantIdAsync(
			request.TenantId,
			request.PageRequest,
			ct);
	}
}
