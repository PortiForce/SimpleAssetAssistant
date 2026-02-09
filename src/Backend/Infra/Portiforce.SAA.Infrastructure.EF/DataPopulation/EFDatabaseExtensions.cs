using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders;
using Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders.Internal;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation;

public static class EFDatabaseExtensions
{
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
				rootTenant = tenants.FirstOrDefault(x => x.Code == "PORTIFORCE");

				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {tenants.Count} Tenants.");
			}
			else
			{
				var tenants = dbContext.Tenants;
				rootTenant = tenants.FirstOrDefault(x => x.Code == "PORTIFORCE");
			}
		}
		catch (Exception ex)
		{
			// since I in static cotext: resolve logger
			// var logger = services.GetRequiredService<ILogger<Program>>();
			// logger.LogError(ex, "Failed to updated global dictionaries");
			throw;
		}

		if (rootTenant != null)
		{
			try
			{
				PlatformAccountSeeder platformAccountsSeeder = services.GetRequiredService<PlatformAccountSeeder>();

				// create Users (Dynamic credentials!)
				List<Account> platformUsers = platformAccountsSeeder.BuildPlatformAccounts(rootTenant);
				dbContext.Accounts.AddRange(platformUsers);
				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {platformUsers.Count} Platform Accounts.");

			}
			catch (Exception ex)
			{
				// since I in static cotext: resolve logger
				// var logger = services.GetRequiredService<ILogger<Program>>();
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