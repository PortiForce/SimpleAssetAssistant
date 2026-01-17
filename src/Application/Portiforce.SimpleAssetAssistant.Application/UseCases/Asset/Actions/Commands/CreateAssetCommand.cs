using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Commands;

public sealed record CreateAssetCommand(
	string Name,
	string Code,
	string AssetKind,
	byte NativeDecimals
) : ICommand<BaseCreateCommandResponse<AssetId>>;
