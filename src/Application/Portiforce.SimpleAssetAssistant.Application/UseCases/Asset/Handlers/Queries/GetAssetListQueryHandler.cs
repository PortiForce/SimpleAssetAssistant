using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Asset;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Queries;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Handlers.Queries;

public sealed class GetAssetListQueryHandler(
	IAssetRepository assetRepository
) : IRequestHandler<GetAssetListRequest, PagedResult<AssetListItemDto>>
{
	public async ValueTask<PagedResult<AssetListItemDto>> Handle(GetAssetListRequest request, CancellationToken ct)
	{
		// 1. Fetch from Repository
		// The Repository returns Domain Entities (or a projection if using EF Core Select)
		// For MVP, assume Repository returns PagedResult<Asset>
		PagedResult<Core.Assets.Models.Asset> pagedAssets = await assetRepository.GetListByTenantIdAsync(
			request.TenantId,
			request.PageRequest,
			ct);

		// 2. Map Entity -> DTO
		// do this manually to avoid AutoMapper magic in the MVP phase.
		List<AssetListItemDto> dtos = pagedAssets.Items.Select(asset => new AssetListItemDto(
			Id: asset.Id,
			Code: asset.Code,
			Name: asset.Name ?? string.Empty,
			Kind: asset.Kind,
			NativeDecimals: asset.NativeDecimals,
			State: asset.EntityState
		)).ToList();

		// 3. Return Result
		return new PagedResult<AssetListItemDto>(
			dtos,
			pagedAssets.TotalCount,
			pagedAssets.PageNumber,
			pagedAssets.PageSize
		);
	}
}
