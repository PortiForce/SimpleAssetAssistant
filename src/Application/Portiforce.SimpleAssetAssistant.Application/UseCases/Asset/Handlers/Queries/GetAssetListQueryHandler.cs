using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Handlers.Queries;

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
