using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders.Internal;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation;

public static class EFDatabaseExtensions
{
	public static async Task PopulateGlobalDictionariesAndPrepareUserAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var services = scope.ServiceProvider;
		var dbContext = services.GetRequiredService<AssetAssistantDbContext>();

		// Apply Schema Migrations First
		await dbContext.Database.MigrateAsync();

		try
		{
			// 1. Assets seeding
			if (!dbContext.Assets.Any())
			{
				Console.WriteLine("Seeding Assets.");
				List<Asset> assets = AssetDataSeeder.GetAssets();
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
				List<Platform> platforms = PlatformDataSeeder.GetPlatforms();
				dbContext.Platforms.AddRange(platforms);
				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {platforms.Count} Platforms.");
			}

			// 3. Tenants:seeding
			if (!dbContext.Tenants.Any())
			{
				Console.WriteLine("Seeding Tenants.");
				List<Tenant> tenants = TenantDataSeeder.GetTenants();
				dbContext.Tenants.AddRange(tenants);
				await dbContext.SaveChangesAsync();
				Console.WriteLine($"Seeded {tenants.Count} Tenants.");
			}
		}
		catch (Exception ex)
		{
			// since I in static cotext: resolve logger
			// var logger = services.GetRequiredService<ILogger<Program>>();
			// logger.LogError(ex, "Failed to updated global dictionaries");
			throw;
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