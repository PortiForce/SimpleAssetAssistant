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

	public List<TenantInvite> BuildPlatformInvites(
		Tenant rootTenant,
		Account sysAccount)
	{
		List<TenantInvite> platformUserInvites = [];

		string ownerToken = tokenGenerator.GenerateInviteToken();
		byte[] ownerTokenHash = hashingService.HashInviteToken(ownerToken);

		TenantInvite platformOwnerInvite = BuildPlatformOwnerInvite(
			rootTenant,
			this._config.PlatformOwner,
			sysAccount,
			ownerTokenHash);

		string adminToken = tokenGenerator.GenerateInviteToken();
		byte[] adminTokenHash = hashingService.HashInviteToken(adminToken);

		TenantInvite platformAdminInvite = BuildPlatformAdminInvite(
			rootTenant,
			this._config.PlatformAdmin,
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
		_ = Enum.TryParse(owner.Tier, out AccountTier accountTier);

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
		_ = Enum.TryParse(admin.Tier, out AccountTier accountTier);

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