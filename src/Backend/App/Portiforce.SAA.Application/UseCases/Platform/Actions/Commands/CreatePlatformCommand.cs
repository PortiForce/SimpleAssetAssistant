using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Platform.Actions.Commands;

public sealed record CreatePlatformCommand(
	string Name,
	string Code,
	PlatformKind Kind 
) : ICommand<CommandResult<PlatformId>>;
