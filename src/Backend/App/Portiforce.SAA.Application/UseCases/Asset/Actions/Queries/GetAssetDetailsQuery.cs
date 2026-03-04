using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Asset.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Asset.Actions.Queries;

public sealed record GetAssetDetailsQuery(AssetId Id) : IQuery<AssetDetails>
{
}
