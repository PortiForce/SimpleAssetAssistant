using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Asset;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Queries;

public sealed record GetAccountDetailsRequest(AssetId Id) : IQuery<AssetDetailsDto>
{
}
