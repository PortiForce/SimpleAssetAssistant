using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;
using Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders.Internal;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation;

public static class EFDatabaseExtensions
{
	private const string DefaultRootTenantCode = "PORTIFORCE";
	private const string DefaultDemoTenantCode = "DEMO";

	private const string PlatformKind = "Platform";
	private const string DemoKind = "Demo";

	public static async Task PopulateGlobalDictionariesAndPrepareUserAsync(this WebApplication app)
	{
		using IServiceScope scope = app.Services.CreateScope();

		IServiceProvider services = scope.ServiceProvider;
		AssetAssistantDbContext dbContext = services.GetRequiredService<AssetAssistantDbContext>();

		// Apply Schema Migrations First
		await dbContext.Database.MigrateAsync();

		Tenant rootTenant;
		Tenant demoTenant;

		try
		{
			// 1. Assets seeding
			if (!dbContext.Assets.Any())
			{
				Console.WriteLine("Seeding Assets.");
				List<Asset> assets = AssetDataSeeder.BuildAssets();
				dbContext.Assets.AddRange(assets);
				_ = await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {assets.Count} Assets.");
			}
			else
			{
				Console.WriteLine("Assets already exist. Skipping seed.");
			}

			// 2. Platforms seeding
			if (!dbContext.Platforms.Any())
			{
				Console.WriteLine("Seeding Platforms.");
				List<Platform> platforms = PlatformDataSeeder.BuildPlatforms();
				dbContext.Platforms.AddRange(platforms);
				_ = await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {platforms.Count} Platforms.");
			}

			// 3. Tenants:seeding
			if (!dbContext.Tenants.Any())
			{
				Console.WriteLine("Seeding Tenants.");
				List<Tenant> tenants = TenantDataSeeder.BuildTenants();
				dbContext.Tenants.AddRange(tenants);
				rootTenant = tenants.FirstOrDefault(x => x.Code.ToUpper() == DefaultRootTenantCode);
				demoTenant = tenants.FirstOrDefault(x => x.Code.ToUpper() == DefaultDemoTenantCode);

				_ = await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {tenants.Count} Tenants.");
			}
			else
			{
				DbSet<Tenant> tenants = dbContext.Tenants;
				rootTenant = tenants.FirstOrDefault(x => x.Code.ToUpper() == DefaultRootTenantCode);
				demoTenant = tenants.FirstOrDefault(x => x.Code.ToUpper() == DefaultDemoTenantCode);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to updated global dictionaries, ex: {ex}");
			throw;
		}

		Account platformSystemAccount = await GetOrCreateSystemAccountAsync(
			rootTenant,
			dbContext,
			services,
			static (seeder, t) => seeder.BuildPlatformSystemAccount(t),
			PlatformKind);

		await GetOrCreateInvitesAsync(
			dbContext,
			services,
			rootTenant,
			static (seeder, t, acc) => seeder.BuildPlatformInvites(t, acc),
			platformSystemAccount,
			PlatformKind);

		Account demoSystemAccount = await GetOrCreateSystemAccountAsync(
			demoTenant,
			dbContext,
			services,
			static (seeder, t) => seeder.BuildDemoSystemAccount(t),
			DemoKind);

		await GetOrCreateInvitesAsync(
			dbContext,
			services,
			demoTenant,
			static (seeder, t, acc) => seeder.BuildDemoInvites(t, acc),
			demoSystemAccount,
			DemoKind);

		DbUserSeeder dbUserSeeder = services.GetRequiredService<DbUserSeeder>();

		// create Users (Dynamic credentials!)
		await dbUserSeeder.CreateServiceUsersIfNotExistAsync();
	}

	private static async Task<Account> GetOrCreateSystemAccountAsync(
		Tenant tenant,
		AssetAssistantDbContext dbContext,
		IServiceProvider services,
		Func<SystemAccountSeeder, Tenant, Account> accountFactory,
		string accountKind,
		CancellationToken ct = default)
	{
		Account? existingAccount = await dbContext.Accounts
			.SingleOrDefaultAsync(
				x => x.Role == Role.TenantBackground && x.TenantId == tenant.Id,
				ct);

		if (existingAccount is not null)
		{
			return existingAccount;
		}

		try
		{
			SystemAccountSeeder seeder = services.GetRequiredService<SystemAccountSeeder>();

			Account systemAccount = accountFactory(seeder, tenant);

			_ = dbContext.Accounts.Add(systemAccount);
			_ = await dbContext.SaveChangesAsync(ct);

			Console.WriteLine($"Seeded {accountKind} system account.");
			return systemAccount;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to populate {accountKind} system account, ex: {ex}");
			throw;
		}
	}

	private static async Task GetOrCreateInvitesAsync(
		AssetAssistantDbContext dbContext,
		IServiceProvider services,
		Tenant tenant,
		Func<InviteSeeder, Tenant, Account, List<TenantInvite>> inviteFactory,
		Account systemAccount,
		string accountKind)
	{
		if (!dbContext.Invites.Any(x => x.TenantId == tenant.Id))
		{
			try
			{
				InviteSeeder seeder = services.GetRequiredService<InviteSeeder>();

				// create invites
				List<TenantInvite> platformUserInvites = inviteFactory(
					seeder,
					tenant,
					systemAccount);

				dbContext.Invites.AddRange(platformUserInvites);
				_ = await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {platformUserInvites.Count} {accountKind} Invites.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to populate accounts, ex: {ex}");
				throw;
			}
		}
	}
}