namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Auth;

internal interface IAuthService
{
	Task RevokeRefreshTokenAsync(string requestRefreshToken, CancellationToken ct);
}
