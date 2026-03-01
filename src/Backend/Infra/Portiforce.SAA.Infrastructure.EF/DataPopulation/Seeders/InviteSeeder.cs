using Microsoft.Extensions.Options;

using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.Auth;
using Portiforce.SAA.Infrastructure.Configuration.Platform;
using Portiforce.SAA.Infrastructure.Services.Security;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;

public class InviteSeeder(IOptions<PlatformUsers> platformUsersOptions)
{
	private readonly PlatformUsers _config = platformUsersOptions.Value;

	public List<TenantInvite> BuildPlatformInvites(
		Tenant rootTenant,
		Account sysAccount,
		JwtTokenGenerator tokenGeneratorService,
		TokenHashingService tokenHashingService)
	{
		var platformUserInvites = new List<TenantInvite>();

		var ownerToken = tokenGeneratorService.GenerateInviteToken();
		var ownerTokenHash = tokenHashingService.HashInviteToken(ownerToken);

		var platformOwnerInvite = BuildPlatformOwnerInvite(
			rootTenant,
			_config.PlatformOwner,
			sysAccount,
			ownerTokenHash);

		var adminToken = tokenGeneratorService.GenerateInviteToken();
		var adminTokenHash = tokenHashingService.HashInviteToken(adminToken);

		var platformAdminInvite = BuildPlatformAdminInvite(
			rootTenant,
			_config.PlatformAdmin,
			sysAccount,
			adminTokenHash);

		platformUserInvites.Add(platformOwnerInvite);
		platformUserInvites.Add(platformAdminInvite);

		return platformUserInvites;
	}

	private static TenantInvite BuildPlatformOwnerInvite(
		Tenant rootTenant,
		PlatformUser owner,
		Account sysAccount,
		byte[] tokenHash)
	{
		Enum.TryParse(owner.Tier, out AccountTier accountTier);

		InviteTarget inviteTarget = InviteTarget.Email(owner.Email);

		return TenantInvite.Create(
			rootTenant.Id,
			inviteTarget,
			sysAccount.Id,
			Role.PlatformOwner,
			accountTier,
			tokenHash,
			DateTimeOffset.Now,
			DateTimeOffset.Now.AddDays(10));
	}

	private static TenantInvite BuildPlatformAdminInvite(
		Tenant rootTenant,
		PlatformUser admin,
		Account sysAccount,
		byte[] tokenHash)
	{
		Enum.TryParse(admin.Tier, out AccountTier accountTier);

		InviteTarget inviteTarget = InviteTarget.Email(admin.Email);

		return TenantInvite.Create(
			rootTenant.Id,
			inviteTarget,
			sysAccount.Id,
			Role.PlatformAdmin,
			accountTier,
			tokenHash,
			DateTimeOffset.Now,
			DateTimeOffset.Now.AddDays(10));
	}
}
