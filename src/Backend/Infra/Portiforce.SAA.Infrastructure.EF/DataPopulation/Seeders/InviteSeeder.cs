using System.Text.RegularExpressions;

using Microsoft.Extensions.Options;

using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.Configuration.Platform;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;

public class InviteSeeder(
	IOptions<PlatformUsers> platformUsersOptions,
	ITokenGenerator tokenGenerator,
	IHashingService hashingService)
{
	private readonly PlatformUsers _config = platformUsersOptions.Value;

	private readonly List<string> unitedStatesPresidents =
	[
		"George Washington",
		"John Adams",
		"Thomas Jefferson",
		"James Madison",
		"James Monroe",
		"John Quincy Adams",
		"Andrew Jackson",
		"Martin Van Buren",
		"William Henry Harrison",
		"John Tyler",
		"James K. Polk",
		"Zachary Taylor",
		"Millard Fillmore",
		"Franklin Pierce",
		"James Buchanan",
		"Abraham Lincoln",
		"Andrew Johnson",
		"Ulysses S. Grant",
		"Rutherford B. Hayes",
		"James A. Garfield",
		"Chester A. Arthur",
		"Grover Cleveland",
		"Benjamin Harrison",
		"Grover Cleveland",
		"William McKinley",
		"Theodore Roosevelt",
		"William Howard Taft",
		"Woodrow Wilson",
		"Warren G. Harding",
		"Calvin Coolidge",
		"Herbert Hoover",
		"Franklin D. Roosevelt",
		"Harry S. Truman",
		"Dwight D. Eisenhower",
		"John F. Kennedy",
		"Lyndon B. Johnson",
		"Richard Nixon",
		"Gerald Ford",
		"Jimmy Carter",
		"Ronald Reagan",
		"George H. W. Bush",
		"Bill Clinton",
		"George W. Bush",
		"Barack Obama",
		"Donald Trump",
		"Joe Biden"
	];

	public List<TenantInvite> BuildPlatformInvites(
		Tenant tenant,
		Account account)
	{
		List<TenantInvite> platformUserInvites = [];

		byte[] ownerTokenHash = this.BuildTokenHash();
		TenantInvite platformOwnerInvite = BuildPlatformOwnerInvite(
			tenant,
			this._config.PlatformOwner,
			account,
			ownerTokenHash);

		byte[] adminTokenHash = this.BuildTokenHash();
		TenantInvite platformAdminInvite = BuildAdminInvite(
			tenant,
			this._config.PlatformAdmin,
			account,
			Role.PlatformAdmin,
			adminTokenHash);

		platformUserInvites.Add(platformOwnerInvite);
		platformUserInvites.Add(platformAdminInvite);

		return platformUserInvites;
	}

	private byte[] BuildTokenHash()
	{
		string token = tokenGenerator.GenerateInviteToken();
		return hashingService.HashInviteToken(token);
	}

	private static TenantInvite BuildPlatformOwnerInvite(
		Tenant rootTenant,
		PlatformUser owner,
		Account sysAccount,
		byte[] tokenHash)
	{
		_ = Enum.TryParse(owner.Tier, out AccountTier accountTier);

		InviteTarget inviteTarget = InviteTarget.Email(owner.Email);

		return TenantInvite.Create(
			rootTenant.Id,
			inviteTarget,
			sysAccount.Id,
			"p-owner",
			Role.PlatformOwner,
			accountTier,
			tokenHash,
			DateTimeOffset.Now,
			DateTimeOffset.Now.AddDays(10));
	}

	public List<TenantInvite> BuildDemoInvites(
		Tenant tenant,
		Account account)
	{
		List<TenantInvite> demoUserInvites = [];

		byte[] adminTokenHash = this.BuildTokenHash();
		TenantInvite demoAdminInvite = BuildAdminInvite(
			tenant,
			this._config.DemoAdmin,
			account,
			Role.TenantAdmin,
			adminTokenHash);

		demoUserInvites.Add(demoAdminInvite);

		int index = 0;
		foreach (string invitePerson in this.unitedStatesPresidents)
		{
			_ = this.BuildTokenHash();
			Random random = new();
			int value = random.Next(-10, 0);

			DateTime dateInviteSent = DateTime.Today.AddDays(value);

			Role role = index % 7 == 0 ? Role.TenantAdmin : Role.TenantUser;

			InviteChannel channel = index % 2 == 0
				? InviteChannel.Email
				: index % 3 == 0
					? InviteChannel.Telegram
					: InviteChannel.AppleAccount;

			InviteTargetKind targetKind = channel switch
			{
				InviteChannel.Email => InviteTargetKind.Email,
				InviteChannel.AppleAccount => index % 2 == 0 ? InviteTargetKind.Email : InviteTargetKind.Phone,
				InviteChannel.Telegram => index % 3 == 0
					? InviteTargetKind.TelegramUserId
					: InviteTargetKind.TelegramUserName
			};

			AccountTier tier = index % 2 == 0
				? AccountTier.Investor
				: index % 6 == 0
					? AccountTier.Observer
					: AccountTier.Strategist;

			InviteTarget inviteTarget = this.BuildInviteTarget(channel, targetKind, invitePerson);

			TenantInvite userInvite = BuildUserInvite(
				tenant,
				account,
				role,
				inviteTarget,
				tier,
				dateInviteSent,
				random.Next(1, 20),
				this.BuildTokenHash());

			if (index % 7 == 1 && userInvite.State != InviteState.Accepted)
			{
				_ = userInvite.Decline(DateTime.Today);
			}

			if (index % 9 == 1 && userInvite.State is not InviteState.DeclinedByUser and not InviteState.Accepted)
			{
				_ = userInvite.Revoke(account.Id, DateTime.Today, false);
			}

			demoUserInvites.Add(userInvite);

			index++;
		}

		return demoUserInvites;
	}

	private InviteTarget BuildInviteTarget(InviteChannel channel, InviteTargetKind targetKind, string invitePerson)
	{
		if (channel == InviteChannel.Email)
		{
			return BuildEmailPart(invitePerson, "gmail-demo.com");
		}

		if (channel == InviteChannel.AppleAccount)
		{
			if (targetKind == InviteTargetKind.Email)
			{
				return BuildEmailPart(invitePerson, "icloud-demo.com");
			}

			if (targetKind == InviteTargetKind.Phone)
			{
				return InviteTarget.ApplePhone($"+1{new Random().Next(100000000, 999999999)}");
			}

			throw new ArgumentException("Invalid target kind for Apple Account channel.");
		}

		if (channel == InviteChannel.Telegram)
		{
			if (targetKind == InviteTargetKind.TelegramUserId)
			{
				return InviteTarget.TelegramUserId($"{new Random().Next(100000000, 999999999)}");
			}

			if (targetKind == InviteTargetKind.TelegramUserName)
			{
				return InviteTarget.TelegramUserName(invitePerson.Replace(" ", string.Empty).ToLower());
			}
		}

		throw new NotSupportedException($"Invite channel is not supported: {channel}");
	}

	private static TenantInvite BuildAdminInvite(
		Tenant tenant,
		PlatformUser admin,
		Account sysAccount,
		Role role,
		byte[] tokenHash)
	{
		_ = Enum.TryParse(admin.Tier, out AccountTier accountTier);

		InviteTarget inviteTarget = InviteTarget.Email(admin.Email);

		return TenantInvite.Create(
			tenant.Id,
			inviteTarget,
			sysAccount.Id,
			$"{tenant.Name}@admin",
			role,
			accountTier,
			tokenHash,
			DateTimeOffset.Now,
			DateTimeOffset.Now.AddDays(10));
	}

	private static TenantInvite BuildUserInvite(
		Tenant tenant,
		Account invitedByAccount,
		Role role,
		InviteTarget inviteTarget,
		AccountTier tier,
		DateTimeOffset createdDate,
		int expiresInDays,
		byte[] tokenHash)
	{
		return TenantInvite.Create(
			tenant.Id,
			inviteTarget,
			invitedByAccount.Id,
			$"{tenant.Name}-{inviteTarget.Value}",
			role,
			tier,
			tokenHash,
			createdDate,
			createdDate.AddDays(expiresInDays));
	}

	private static InviteTarget BuildEmailPart(string personName, string domainName)
	{
		string localPart = Regex.Replace(personName.Trim().ToLowerInvariant(), @"[^a-z0-9]+", ".");
		localPart = Regex.Replace(localPart, @"\.+", ".").Trim('.');

		return InviteTarget.Email($"{localPart}@{domainName}");
	}
}