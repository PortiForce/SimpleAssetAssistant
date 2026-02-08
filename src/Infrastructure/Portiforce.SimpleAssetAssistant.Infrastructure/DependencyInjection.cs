using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Models.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.Configuration;
using Portiforce.SimpleAssetAssistant.Infrastructure.Configuration.Platform;

namespace Portiforce.SimpleAssetAssistant.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHttpContextAccessor();

		services.AddScoped<IGoogleAuthProvider, GoogleAuthProvider>();
		services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
		services.AddScoped<ICurrentUser, CurrentUser>();

		// infrastructure settings
		services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
		services.Configure<GoogleClientSettings>(configuration.GetSection("GoogleClientSettings"));

		// data operation settings
		services.Configure<PlatformUsersSettings>(configuration.GetSection("PlatformUsers"));

		return services;
	}
}