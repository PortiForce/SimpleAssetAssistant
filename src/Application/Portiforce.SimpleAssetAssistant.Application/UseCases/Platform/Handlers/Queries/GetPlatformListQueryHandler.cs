using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Handlers.Queries;

public sealed class GetPlatformListQueryHandler(
	IPlatformReadRepository platformRepository
) : IRequestHandler<GetPlatformListQuery, PagedResult<PlatformListItem>>
{
	public async ValueTask<PagedResult<PlatformListItem>> Handle(GetPlatformListQuery request, CancellationToken ct)
	{
		// 1. Fetch from Repository
		// The Repository returns Projections directly
		PagedResult<PlatformListItem> pagedAssets = await platformRepository.GetListByTenantIdAsync(
			request.TenantId,
			request.PageRequest,
			ct);

		return pagedAssets;
	}
}
