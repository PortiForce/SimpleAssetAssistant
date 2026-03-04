
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Core.Identity.Models.Client;

public sealed record TenantDefaults
{
	public FiatCurrency DefaultCurrency { get; init; } = FiatCurrency.GBP;
	public string DefaultLocale { get; init; } = "en-GB";
	public string DefaultTimeZoneId { get; init; } = "UTC";

	public static TenantDefaults Default() => new();
}