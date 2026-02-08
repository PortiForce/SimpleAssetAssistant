namespace Portiforce.SimpleAssetAssistant.Application.Models.Auth;

public record GoogleUserInfo(
	string Email,
	string ExternalId,
	string? Name,
	string? PictureUrl);
