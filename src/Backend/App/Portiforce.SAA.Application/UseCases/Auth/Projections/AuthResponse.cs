namespace Portiforce.SAA.Application.UseCases.Auth.Projections;

public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
