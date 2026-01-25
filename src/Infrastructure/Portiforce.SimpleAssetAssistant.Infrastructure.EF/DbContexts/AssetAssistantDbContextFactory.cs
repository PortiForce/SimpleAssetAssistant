using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

public sealed class AssetAssistantDbContextFactory
	: IDesignTimeDbContextFactory<AssetAssistantDbContext>
{
	public AssetAssistantDbContext CreateDbContext(string[] args)
	{
		var cs = Environment.GetEnvironmentVariable("ASSETASSISTANT_DB");

		if (string.IsNullOrWhiteSpace(cs))
		{
			throw new InvalidOperationException(
				"Missing connection string. Set environment variable ASSETASSISTANT_DB.");
		}

		var options = new DbContextOptionsBuilder<AssetAssistantDbContext>()
			.UseSqlServer(cs, sql =>
			{
				// ensures migrations are created in this EF project
				sql.MigrationsAssembly(typeof(AssetAssistantDbContext).Assembly.FullName);
				
				// for local Docker SQL Server speed
				sql.EnableRetryOnFailure();
			})
			.Options;

		return new AssetAssistantDbContext(options);
	}
}