using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Portiforce.SAA.Infrastructure.Configuration.Platform;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.DataPopulation.Seeders.Internal;

public class DbUserSeeder(
	AssetAssistantDbContext dbContext,
	IOptions<PlatformUsers> platformUsersOptions,
	ILogger<DbUserSeeder> logger)
{
	private readonly PlatformUsers _config = platformUsersOptions.Value;

	public async Task CreateServiceUsersIfNotExistAsync()
	{
		SqlPlatformUser userConfig = this._config.SqlPlatformUser;
		if (string.IsNullOrEmpty(userConfig.Login) || string.IsNullOrEmpty(userConfig.Password))
		{
			logger.LogWarning("Skipping SQL User creation: Credentials missing in config.");
			return;
		}

		try
		{
			// 1. Create Server Login
			string createLoginSql = $@"
                IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = '{userConfig.Login}')
                BEGIN
                    CREATE LOGIN [{userConfig.Login}] WITH PASSWORD = '{userConfig.Password}', CHECK_POLICY = ON;
                END";

			_ = await dbContext.Database.ExecuteSqlRawAsync(createLoginSql);

			// 2. Create Database User
			string createUserSql = $@"
                IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = '{userConfig.Login}')
                BEGIN
                    CREATE USER [{userConfig.Login}] FOR LOGIN [{userConfig.Login}];
                END";

			_ = await dbContext.Database.ExecuteSqlRawAsync(createUserSql);

			// 3. Assign Roles
			string assignRolesSql = $@"
                ALTER ROLE [db_datareader] ADD MEMBER [{userConfig.Login}];
                ALTER ROLE [db_datawriter] ADD MEMBER [{userConfig.Login}];
            ";

			_ = await dbContext.Database.ExecuteSqlRawAsync(assignRolesSql);

			logger.LogInformation("SQL Service Account '{Login}' verified/created successfully.", userConfig.Login);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to create SQL Service Account.");
			throw;
		}
	}
}