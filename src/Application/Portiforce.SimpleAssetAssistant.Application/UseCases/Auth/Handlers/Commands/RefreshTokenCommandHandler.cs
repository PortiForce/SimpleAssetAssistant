using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Auth;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Handlers.Commands;

internal sealed class RefreshTokenCommandHandler(IAuthService authService)
	: IRequestHandler<RefreshTokenCommand, AuthResponse>
{
	public async ValueTask<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			throw new ArgumentException("Refresh token is required.", nameof(request.RefreshToken));
		}

		// Classic pattern: no current user context, so authService must validate ownership
		// by looking up token record and binding it to a user/tenant internally.
		return await authService.RefreshTokenAsync(
			request.RefreshToken,
			request.IpAddress,
			request.UserAgent,
			ct);
	}
}
