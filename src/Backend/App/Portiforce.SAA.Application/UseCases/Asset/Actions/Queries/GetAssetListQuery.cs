using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Asset.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Asset.Actions.Queries;

public sealed record GetAssetListQuery(
	TenantId TenantId,
	PageRequest PageRequest,
	string? SearchTerm
) : IQuery<PagedResult<AssetListItem>>;
