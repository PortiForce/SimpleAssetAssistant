using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts;

/// <summary>
///     A factory to be able to run migrations from command line
/// </summary>
public sealed class AssetAssistantDbContextFactory
	: IDesignTimeDbContextFactory<AssetAssistantDbContext>
{
	public AssetAssistantDbContext CreateDbContext(string[] args)
	{
		string? cs = Environment.GetEnvironmentVariable("ASSETASSISTANT_DB");

		if (string.IsNullOrWhiteSpace(cs))
		{
			throw new InvalidOperationException(
				"Missing connection string. Set environment variable ASSETASSISTANT_DB.");
		}

		DbContextOptions<AssetAssistantDbContext> options = new DbContextOptionsBuilder<AssetAssistantDbContext>()
			.UseSqlServer(
				cs,
				sql =>
				{
					// ensures migrations are created in this EF project
					_ = sql.MigrationsAssembly(typeof(AssetAssistantDbContext).Assembly.FullName);

					// for local Docker SQL Server speed
					_ = sql.EnableRetryOnFailure();
				})
			.Options;

		return new AssetAssistantDbContext(options);
	}
}