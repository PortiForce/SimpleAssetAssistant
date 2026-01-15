using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Actions.Commands;

public sealed record CreatePlatformCommand(
	string Name,
	string Code,              // e.g. "binance_global"
	string Kind               // "Exchange", "Wallet"
) : ICommand<BaseCreateCommandResponse<PlatformId>>;
