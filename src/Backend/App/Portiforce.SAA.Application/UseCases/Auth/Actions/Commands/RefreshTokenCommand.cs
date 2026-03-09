using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Projections;

namespace Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;

public sealed record RefreshTokenCommand(
	string RefreshToken,
	string IpAddress,
	string UserAgent) : ICommand<AuthResponse>;
