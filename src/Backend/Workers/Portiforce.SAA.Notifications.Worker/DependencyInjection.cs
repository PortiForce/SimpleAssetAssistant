using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Notifications.Worker.Invite;
using Portiforce.SAA.Notifications.Worker.Services;

namespace Portiforce.SAA.Notifications.Worker;

public static class DependencyInjection
{
	public static IServiceCollection AddNotificationsInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		_ = services.AddScoped<IInviteLinkBuilder, InviteLinkBuilder>();
		_ = services.AddScoped<IInviteChannelSender, EmailInviteChannelSender>();

		return services;
	}
}