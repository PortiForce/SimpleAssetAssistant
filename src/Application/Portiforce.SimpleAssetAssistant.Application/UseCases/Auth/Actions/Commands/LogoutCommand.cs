using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;

public sealed record LogoutCommand(string RefreshToken) : ICommand<Unit>;

