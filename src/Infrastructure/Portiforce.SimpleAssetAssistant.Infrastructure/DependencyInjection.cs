using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.Configuration;
using Portiforce.SimpleAssetAssistant.Infrastructure.Configuration.Platform;

namespace Portiforce.SimpleAssetAssistant.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IGoogleAuthProvider, GoogleAuthProvider>();
		services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

		// infrastructure settings
		services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
		services.Configure<GoogleClientSettings>(configuration.GetSection("GoogleClientSettings"));

		// data operation settings
		services.Configure<PlatformUsersSettings>(configuration.GetSection("PlatformUsers"));

		//services.AddIdentity<ApplicationUser, IdentityRole>()
		//	.AddEntityFrameworkStores<CustomerHubIdentityDbContext>().AddDefaultTokenProviders();

		//services.AddTransient<IAuthService, AuthService>();

		//services.AddAuthentication(options =>
		//	{
		//		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		//		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		//	})
		//	.AddJwtBearer(o =>
		//	{
		//		o.TokenValidationParameters = new TokenValidationParameters
		//		{
		//			ValidateIssuerSigningKey = true,
		//			ValidateIssuer = true,
		//			ValidateAudience = true,
		//			ValidateLifetime = true,
		//			ClockSkew = TimeSpan.Zero,
		//			ValidIssuer = configuration["JwtSettings:Issuer"],
		//			ValidAudience = configuration["JwtSettings:Audience"],
		//			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
		//		};
		//	});

		return services;
	}
}