using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var asm = typeof(DependencyInjection).Assembly;

		RegisterHandlers(services, asm);

		// Mediator + publisher
		services.AddScoped<IMediator, Mediator>();
		services.AddScoped<IPublisher>(sp => (IPublisher)sp.GetRequiredService<IMediator>());

		// behaviors (add later)
		// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

		return services;
	}

	private static void RegisterHandlers(IServiceCollection services, Assembly asm)
	{
		var types = asm.DefinedTypes.Where(t => t is { IsAbstract: false, IsInterface: false });

		foreach (var t in types)
		{
			foreach (var itf in t.ImplementedInterfaces)
			{
				if (!itf.IsGenericType) continue;

				var def = itf.GetGenericTypeDefinition();
				if (def == typeof(IRequestHandler<,>) || def == typeof(INotificationHandler<>))
				{
					services.AddTransient(itf, t);
				}
			}
		}
	}
}
