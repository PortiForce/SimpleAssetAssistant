using Portiforce.SAA.Application.Interfaces.Persistence.Asset;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Asset.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Asset.Projections;

namespace Portiforce.SAA.Application.UseCases.Asset.Handlers.Queries;

public sealed class GetAssetListQueryHandler(
	IAssetReadRepository assetRepository
) : IRequestHandler<GetAssetListQuery, PagedResult<AssetListItem>>
{
	public async ValueTask<PagedResult<AssetListItem>> Handle(GetAssetListQuery request, CancellationToken ct)
	{
		// 1. Fetch from Repository
		// The Repository returns Projections directly
		PagedResult<AssetListItem> pagedAssets = await assetRepository.GetListByTenantIdAsync(
			request.TenantId,
			request.PageRequest,
			ct);

		return pagedAssets;
	}
}
