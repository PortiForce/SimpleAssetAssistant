using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Auth;

internal interface IAuthService
{
	Task<AuthResponse> RefreshTokenAsync(
		string requestRefreshToken,
		string? ipAddress,
		string? userAgent,
		CancellationToken ct);

	Task RevokeRefreshTokenAsync(
		string requestRefreshToken,
		string? ip,
		CancellationToken ct);
}
