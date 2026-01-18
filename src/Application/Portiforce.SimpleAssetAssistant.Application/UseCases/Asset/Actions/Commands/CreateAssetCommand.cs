using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Commands;

public sealed record CreateAssetCommand(
	string Name,
	AssetCode Code,
	AssetKind AssetKind,
	byte NativeDecimals
) : ICommand<BaseCreateCommandResponse<AssetId>>;
