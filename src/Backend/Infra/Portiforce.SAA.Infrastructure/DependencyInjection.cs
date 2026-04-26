using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Abstractions.Serialization;
using Portiforce.SAA.Infrastructure.Auth;
using Portiforce.SAA.Infrastructure.Configuration;
using Portiforce.SAA.Infrastructure.Configuration.Platform;
using Portiforce.SAA.Infrastructure.Invite;
using Portiforce.SAA.Infrastructure.Serialization;

namespace Portiforce.SAA.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		_ = services.AddHttpContextAccessor();

		_ = services.AddScoped<IGoogleAuthProvider, GoogleAuthProvider>();
		_ = services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
		_ = services.AddScoped<ICurrentUser, CurrentUser>();
		_ = services.AddScoped<IInviteLinkBuilder, InviteLinkBuilder>();
		_ = services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();

		// infrastructure settings
		_ = services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
		_ = services.Configure<GoogleClientSettings>(configuration.GetSection("GoogleClientSettings"));
		_ = services.Configure<TokenHashingOptions>(configuration.GetSection("TokenHashingOptions"));
		_ = services.Configure<InviteLinkOptions>(configuration.GetSection("InviteLinkOptions"));
		_ = services.Configure<InviteEmailOptions>(configuration.GetSection("InviteEmailOptions"));

		// data operation settings
		_ = services.Configure<PlatformUsers>(configuration.GetSection("PlatformUsers"));

		return services;
	}
}
