namespace Portiforce.SAA.Application.Models.Auth;

public record GoogleUserInfo(
	string Email,
	string ExternalId,
	string? Name,
	string? PictureUrl);
