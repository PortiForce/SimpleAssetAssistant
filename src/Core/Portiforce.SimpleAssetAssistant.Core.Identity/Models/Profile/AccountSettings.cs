using Portiforce.SimpleAssetAssistant.Core.Primitives;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

public sealed record AccountSettings
{
	public FiatCurrency DefaultCurrency { get; init; } = FiatCurrency.USD;

	public string Locale { get; init; } = "en-GB";

	/// <summary>
	/// Time zone id (as string for now).
	/// </summary>
	public string TimeZoneId { get; init; } = "UTC";

	/// <summary>
	/// User preference; enforcement is a tenant policy.
	/// </summary>
	public bool TwoFactorPreferred { get; init; } = true;

	public static AccountSettings Default() => new();
}