using Portiforce.SimpleAssetAssistant.Core.Primitives;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Requests;

public sealed record UpdateProfileRequest(
	string Alias,
	string? PhoneNumber,
	string? BackupEmail,
	string Locale,
	string DefaultCurrency);
