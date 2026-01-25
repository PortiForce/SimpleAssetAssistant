using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation;

public static class EFDatabaseExtensions
{
	public static void PopulateGlobalDictionaries(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			try
			{
				var context = services.GetRequiredService<AssetAssistantDbContext>();

				// 1. Assets seeding
				if (!context.Assets.Any())
				{
					Console.WriteLine("Seeding Assets.");
					List<Asset> assets = AssetDataSeeder.GetAssets();
					context.Assets.AddRange(assets);
					context.SaveChanges();
					Console.WriteLine($"Seeded {assets.Count} Assets.");
				}
				else
				{
					Console.WriteLine("Assets already exist. Skipping seed.");
				}

				// 2. Platforms seeding
				if (!context.Platforms.Any())
				{
					Console.WriteLine("Seeding Platforms.");
					List<Platform> platforms = PlatformDataSeeder.GetPlatforms();
					context.Platforms.AddRange(platforms);
					context.SaveChanges();
					Console.WriteLine($"Seeded {platforms.Count} Platforms.");
				}

				// 3. Tenants:seeding
				if (!context.Tenants.Any())
				{
					Console.WriteLine("Seeding Tenants.");
					List<Tenant> tenants = TenantDataSeeder.GetTenants();
					context.Tenants.AddRange(tenants);
					context.SaveChanges();
					Console.WriteLine($"Seeded {tenants.Count} Tenants.");
				}
			}
			catch (Exception ex)
			{
				// todo tech: log exception - decide what to do later
				throw;
			}
		}
	}
}