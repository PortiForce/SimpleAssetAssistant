using Microsoft.Extensions.Options;

using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Infrastructure.Configuration.Platform;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;

public class PlatformAccountSeeder(IOptions<PlatformUsers> platformUsersOptions)
{
	private readonly PlatformUsers _config = platformUsersOptions.Value;

	public List<Account> BuildPlatformAccounts(Tenant rootTenant)
	{
		var platformUsers = new List<Account>();

		var platformOwner = BuildPlatformOwner(rootTenant, _config.PlatformOwner);
		var platformAdmin = BuildPlatformAdmin(rootTenant, _config.PlatformAdmin);

		platformUsers.Add(platformOwner);
		platformUsers.Add(platformAdmin);

		return platformUsers;
	}

	private static Account BuildPlatformOwner(Tenant rootTenant, PlatformUser owner)
	{
		Enum.TryParse(owner.Tier, out AccountTier accountTier);
		
		return Account.Create(
			rootTenant.Id,
			owner.Alias,
			new ContactInfo(Email.Create(owner.Email)),
			Role.PlatformOwner,
			AccountState.Active,
			accountTier,
			settings: new AccountSettings
			{
				DefaultCurrency = FiatCurrency.GBP,
				Locale = "en-GB",
				TimeZoneId = "UTC",
				TwoFactorPreferred = true
			});
	}

	private static Account BuildPlatformAdmin(Tenant rootTenant, PlatformUser admin)
	{
		Enum.TryParse(admin.Tier, out AccountTier accountTier);

		return Account.Create(
			rootTenant.Id,
			admin.Alias,
			new ContactInfo(Email.Create(admin.Email)),
			Role.PlatformAdmin,
			AccountState.Active,
			accountTier,
			settings: new AccountSettings
			{
				DefaultCurrency = FiatCurrency.EUR,
				Locale = "en-GB",
				TimeZoneId = "UTC",
				TwoFactorPreferred = true
			});
	}
}
