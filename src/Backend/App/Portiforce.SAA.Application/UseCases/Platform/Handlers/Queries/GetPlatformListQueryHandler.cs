using Portiforce.SAA.Application.Interfaces.Persistence.Platform;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Platform.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Platform.Projections;

namespace Portiforce.SAA.Application.UseCases.Platform.Handlers.Queries;

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
