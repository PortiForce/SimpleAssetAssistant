using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;

public sealed record RefreshTokenCommand(
	string RefreshToken,
	string IpAddress,
	string UserAgent) : ICommand<AuthResponse>;
