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

	public Account BuildPlatformSystemAccount(Tenant rootTenant)
	{
		Enum.TryParse(_config.PlatformBackground.Tier, out AccountTier accountTier);

		return Account.Create(
			rootTenant.Id,
			_config.PlatformBackground.Alias,
			new ContactInfo(Email.Create(_config.PlatformBackground.Email)),
			Role.TenantBackground,
			AccountState.Active,
			accountTier,
			settings: new AccountSettings
			{
				DefaultCurrency = FiatCurrency.UAH,
				Locale = "uk-UA",
				TimeZoneId = "UTC",
				TwoFactorPreferred = true
			});
	}
}
