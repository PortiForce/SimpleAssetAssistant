using Microsoft.Extensions.DependencyInjection;

namespace Portiforce.SimpleAssetAssistant.Core.Identity;

public static class DependencyInjection
{
	public static IServiceCollection AddIdentity(this IServiceCollection services)
	{
		//services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

		//services.AddDbContext<CustomerHubIdentityDbContext>(options => options.UseSqlServer(
		//	configuration.GetConnectionString("CustomerHubIdentityConnectionString"),
		//	set =>
		//	{
		//		set.MigrationsAssembly(typeof(CustomerHubIdentityDbContext).Assembly.FullName);
		//	}));

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