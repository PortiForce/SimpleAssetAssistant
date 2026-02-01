using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.PlatformAccount;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders.Internal;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Activity;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Asset;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Client;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Platform;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.PlatformAccount;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Profile;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF;

public static class DependencyInjection
{
	public static IServiceCollection AddEfInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<AssetAssistantDbContext>(opt =>
		{
			opt.UseSqlServer(
				configuration.GetConnectionString("AssetAssistantSQLDb"),
				sql => sql.MigrationsAssembly(typeof(AssetAssistantDbContext).Assembly.FullName));
			opt.EnableDetailedErrors();

#if DEBUG
			// only for local debugging
			opt.EnableSensitiveDataLogging();
#endif
		});

		services.AddScoped<IUnitOfWork, EfUnitOfWork>();

		// Guards/services that are implemented in infrastructure (if any)
		// if alternative to app-layer guard is required, use this:
		// services.AddScoped<IActivityIdempotencyGuard, ActivityIdempotencyGuardEf>(); 

		// Repositories
		services.AddScoped<ITenantReadRepository, TenantReadRepository>();
		services.AddScoped<ITenantWriteRepository, TenantWriteRepository>();

		services.AddScoped<IAccountReadRepository, AccountReadRepository>();
		services.AddScoped<IAccountWriteRepository, AccountWriteRepository>();

		services.AddScoped<ICurrentUserReadRepository, CurrentUserReadRepository>();
		services.AddScoped<ICurrentUserWriteRepository, CurrentUserWriteRepository>();

		services.AddScoped<IPlatformReadRepository, PlatformReadRepository>();
		services.AddScoped<IPlatformWriteRepository, PlatformWriteRepository>();

		services.AddScoped<IPlatformAccountReadRepository, PlatformAccountReadRepository>();
		services.AddScoped<IPlatformAccountWriteRepository, PlatformAccountWriteRepository>();

		services.AddScoped<IAssetReadRepository, AssetReadRepository>();
		services.AddScoped<IAssetWriteRepository, AssetWriteRepository>();

		services.AddScoped<IActivityReadRepository, ActivityReadRepository>();
		services.AddScoped<IActivityWriteRepository, ActivityWriteRepository>();

		services.AddScoped<IExternalIdentityReadRepository, ExternalIdentityReadRepository>();
		services.AddScoped<IExternalIdentityWriteRepository, ExternalIdentityWriteRepository>();

		// data seeders:
		services.AddScoped<DbUserSeeder>();

		return services;
	}
}