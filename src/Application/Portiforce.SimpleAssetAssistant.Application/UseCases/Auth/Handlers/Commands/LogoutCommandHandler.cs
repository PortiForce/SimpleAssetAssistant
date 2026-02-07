using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Auth;
using Portiforce.SimpleAssetAssistant.Application.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Handlers.Commands;

internal sealed class LogoutCommandHandler(
	IAuthService authService,
	ICurrentUser currentUser) : IRequestHandler<LogoutCommand, Unit>
{
	public async ValueTask<Unit> Handle(LogoutCommand request, CancellationToken ct)
	{
		// 1. Revoke the specific refresh token
		await authService.RevokeRefreshTokenAsync(request.RefreshToken, request.IpAddress, ct);

		// 2. todo: Audit log that User X logged out
		// logger.LogInformation("User {Id} logged out", currentUser.Id);

		return new Unit();
	}
}
