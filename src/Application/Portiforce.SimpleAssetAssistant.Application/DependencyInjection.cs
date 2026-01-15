using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var asm = typeof(DependencyInjection).Assembly;

		// 1. Register Handlers (Commands, Queries, Notifications)
		RegisterHandlers(services, asm);

		// 2. Register Mediator & Publisher
		// Register the concrete class first as Scoped
		services.AddScoped<Mediator>();

		// Forward interfaces to the concrete instance
		// This ensures IMediator and IPublisher are the SAME instance per request
		services.AddScoped<IMediator>(sp => sp.GetRequiredService<Mediator>());
		services.AddScoped<IPublisher>(sp => sp.GetRequiredService<Mediator>());

		// 3. Behaviors (Pipelines)
		// Note: Order matters here. Behaviors execute in the order registered.
		// services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		// services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}

	private static void RegisterHandlers(IServiceCollection services, Assembly asm)
	{
		// Filter for non-abstract, non-interface classes
		var types = asm.DefinedTypes
			.Where(t => t is { IsAbstract: false, IsInterface: false });

		foreach (var type in types)
		{
			foreach (var itf in type.ImplementedInterfaces)
			{
				if (!itf.IsGenericType)
				{
					continue;
				}

				var def = itf.GetGenericTypeDefinition();

				// Register Command/Query Handlers and Notification Handlers
				if (def == typeof(IRequestHandler<,>) || def == typeof(INotificationHandler<>))
				{
					// CHANGE: Use AddScoped instead of AddTransient
					services.AddScoped(itf, type);
				}
			}
		}
	}
}
