using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Asset.Actions.Commands;

public sealed record CreateAssetCommand(
	string Name,
	AssetCode Code,
	AssetKind AssetKind,
	byte NativeDecimals
) : ICommand<CommandResult<AssetId>>;
