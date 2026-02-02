using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Handlers.Queries;

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
