using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.Auth;
using Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;
using Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders.Internal;
using Portiforce.SAA.Infrastructure.EF.DbContexts;
using Portiforce.SAA.Infrastructure.Services.Security;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation;

public static class EFDatabaseExtensions
{
	private const string DefaultRootTenantCode = "PORTIFORCE";

	public static async Task PopulateGlobalDictionariesAndPrepareUserAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var services = scope.ServiceProvider;
		var dbContext = services.GetRequiredService<AssetAssistantDbContext>();

		// Apply Schema Migrations First
		await dbContext.Database.MigrateAsync();

		Tenant? rootTenant = null;

		try
		{
			// 1. Assets seeding
			if (!dbContext.Assets.Any())
			{
				Console.WriteLine("Seeding Assets.");
				List<Asset> assets = AssetDataSeeder.BuildAssets();
				dbContext.Assets.AddRange(assets);
				await dbContext.SaveChangesAsync();
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
				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {platforms.Count} Platforms.");
			}

			// 3. Tenants:seeding
			if (!dbContext.Tenants.Any())
			{
				Console.WriteLine("Seeding Tenants.");
				List<Tenant> tenants = TenantDataSeeder.BuildTenants();
				dbContext.Tenants.AddRange(tenants);
				rootTenant = tenants.FirstOrDefault(x => x.Code.ToUpper() == DefaultRootTenantCode);

				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {tenants.Count} Tenants.");
			}
			else
			{
				var tenants = dbContext.Tenants;
				rootTenant = tenants.FirstOrDefault(x => x.Code.ToUpper() == DefaultRootTenantCode);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to updated global dictionaries, ex: {ex}");
			throw;
		}

		Account platformSystemAccount;
		if (rootTenant != null && !dbContext.Accounts.Any(x => x.TenantId == rootTenant.Id))
		{
			try
			{
				SystemAccountSeeder platformAccountsSeeder = services.GetRequiredService<SystemAccountSeeder>();

				platformSystemAccount = platformAccountsSeeder.BuildPlatformSystemAccount(rootTenant);
				dbContext.Accounts.Add(platformSystemAccount);
				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded Platform System Account.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to populate platform system account, ex: {ex}");
				throw;
			}
		}
		else
		{
			platformSystemAccount = dbContext.Accounts.Single(x => x.Alias == "SYS" && x.TenantId == rootTenant.Id);
		}

		if (rootTenant != null && !dbContext.Invites.Any(x => x.TenantId == rootTenant.Id))
		{
			try
			{
				InviteSeeder platformAccountsSeeder = services.GetRequiredService<InviteSeeder>();
				JwtTokenGenerator tokenGeneratorService = services.GetRequiredService<JwtTokenGenerator>();
				TokenHashingService tokenHashingService = services.GetRequiredService<TokenHashingService>();

				// create invites 
				List<TenantInvite> platformUserInvites = platformAccountsSeeder.BuildPlatformInvites(
					rootTenant,
					platformSystemAccount,
					tokenGeneratorService,
					tokenHashingService);

				dbContext.Invites.AddRange(platformUserInvites);
				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {platformUserInvites.Count} Platform Invites.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to populate accounts, ex: {ex}");
				throw;
			}
		}

		try
		{
			DbUserSeeder dbUserSeeder = services.GetRequiredService<DbUserSeeder>();

			// create Users (Dynamic credentials!)
			await dbUserSeeder.CreateServiceUsersIfNotExistAsync();

		}
		catch (Exception ex)
		{
			// since I in static cotext: resolve logger
			// var logger = services.GetRequiredService<ILogger<Program>>();
			throw;
		}
	}
}