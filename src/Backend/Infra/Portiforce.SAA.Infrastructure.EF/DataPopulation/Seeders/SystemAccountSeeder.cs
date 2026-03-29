using Microsoft.Extensions.Options;

using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Infrastructure.Configuration.Platform;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;

public class SystemAccountSeeder(IOptions<PlatformUsers> platformUsersOptions)
{
	private readonly PlatformUsers _config = platformUsersOptions.Value;

	public Account BuildPlatformSystemAccount(Tenant tenant)
	{
		_ = Enum.TryParse(this._config.PlatformBackground.Tier, out AccountTier accountTier);

		return Account.Create(
			tenant.Id,
			this._config.PlatformBackground.Alias,
			new ContactInfo(Email.Create(this._config.PlatformBackground.Email)),
			Role.TenantBackground,
			AccountState.Suspended,
			accountTier,
			new AccountSettings
			{
				DefaultCurrency = FiatCurrency.UAH,
				Locale = "uk-UA",
				TimeZoneId = "Europe/Kyiv",
				TwoFactorPreferred = true
			});
	}

	public Account BuildDemoSystemAccount(Tenant tenant)
	{
		_ = Enum.TryParse(this._config.PlatformBackground.Tier, out AccountTier accountTier);

		return Account.Create(
			tenant.Id,
			this._config.DemoBackground.Alias,
			new ContactInfo(Email.Create(this._config.DemoBackground.Email)),
			Role.TenantBackground,
			AccountState.Suspended,
			accountTier,
			new AccountSettings
			{
				DefaultCurrency = FiatCurrency.GBP,
				Locale = "en-GB",
				TimeZoneId = "Europe/London",
				TwoFactorPreferred = true
			});
	}
}