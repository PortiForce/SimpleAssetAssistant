namespace Portiforce.SAA.Api.Contracts.Profile.Account.Requests;

public sealed record UpdateProfileRequest(
	string Alias,
	string? PhoneNumber,
	string? BackupEmail,
	string Locale,
	string DefaultCurrency);
