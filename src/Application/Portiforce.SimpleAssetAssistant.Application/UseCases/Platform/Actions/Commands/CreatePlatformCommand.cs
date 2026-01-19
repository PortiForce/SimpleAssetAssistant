using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Actions.Commands;

public sealed record CreatePlatformCommand(
	string Name,
	string Code,
	PlatformKind Kind 
) : ICommand<BaseCreateCommandResult<PlatformId>>;
