using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Infrastructure.Auth;
using Portiforce.SAA.Infrastructure.Configuration;
using Portiforce.SAA.Infrastructure.Configuration.Platform;

namespace Portiforce.SAA.Infrastructure;

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
		services.Configure<PlatformUsers>(configuration.GetSection("PlatformUsers"));

		return services;
	}
}