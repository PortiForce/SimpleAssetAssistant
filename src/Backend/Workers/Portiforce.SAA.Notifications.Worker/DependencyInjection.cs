using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Portiforce.SAA.Application.Interfaces.Messaging;
using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Notifications.Worker.Invite;

namespace Portiforce.SAA.Notifications.Worker;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = services.AddScoped<IInviteChannelSender, EmailInviteChannelSender>();
        _ = services.AddScoped<IInviteNotificationOutboxProcessor, InviteNotificationOutboxProcessor>();

        return services;
    }
}