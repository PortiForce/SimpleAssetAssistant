using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Queries;

public sealed record GetAssetDetailsQuery(AssetId Id) : IQuery<AssetDetails>
{
}
