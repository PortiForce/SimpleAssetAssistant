using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Queries;

public sealed record GetAssetListQuery(
	TenantId TenantId,
	PageRequest PageRequest,
	string? SearchTerm
) : IQuery<PagedResult<AssetListItem>>;
