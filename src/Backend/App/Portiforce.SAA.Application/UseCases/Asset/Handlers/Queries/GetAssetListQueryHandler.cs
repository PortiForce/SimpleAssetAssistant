using Portiforce.SAA.Application.Interfaces.Persistence.Asset;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Asset.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Asset.Projections;

namespace Portiforce.SAA.Application.UseCases.Asset.Handlers.Queries;

public sealed class GetAssetListQueryHandler(
	IAssetReadRepository assetReadRepository
) : IRequestHandler<GetAssetListQuery, PagedResult<AssetListItem>>
{
	public async ValueTask<PagedResult<AssetListItem>> Handle(GetAssetListQuery request, CancellationToken ct)
	{
		PagedResult<AssetListItem> pagedAssets = await assetReadRepository.GetListByTenantIdAsync(
			request.TenantId,
			request.PageRequest,
			ct);

		return pagedAssets;
	}
}
