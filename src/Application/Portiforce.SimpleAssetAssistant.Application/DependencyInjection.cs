using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Portiforce.SimpleAssetAssistant.Application.Entitlements.Resolvers;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Time;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Guards;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Resolvers;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Activity;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Asset;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Tenant;
using Portiforce.SimpleAssetAssistant.Application.Tech.Common.Time;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Guards;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Services;
using Portiforce.SimpleAssetAssistant.Application.UseCases.SharedFlow.Services;

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

		// 3. Remaining services registration
		// Core tech
		services.AddSingleton<IClock, SystemClock>();
		services.AddSingleton<ITenantEntitlementsResolver, DefaultTenantEntitlementsResolver>();

		// UseCases: guards/services
		services.AddScoped<IActivityIdempotencyGuard, ActivityIdempotencyGuard>();
		services.AddScoped<IActivityPersistenceService, ActivityPersistenceService>();
		services.AddScoped<IAssetLookupService, AssetLookupService>();
		services.AddScoped<ITenantLimitsService, TenantLimitsService>();

		return services;
	}

	private static void RegisterHandlers(IServiceCollection services, Assembly asm)
	{
		// Filter for non-abstract, non-interface classes
		IEnumerable<TypeInfo> types = asm.DefinedTypes
			.Where(t => t is { IsAbstract: false, IsInterface: false });

		foreach (TypeInfo type in types)
		{
			foreach (Type itf in type.ImplementedInterfaces)
			{
				if (!itf.IsGenericType)
				{
					continue;
				}

				Type def = itf.GetGenericTypeDefinition();

				// Register Command/Query Handlers and Notification Handlers
				if (def == typeof(IRequestHandler<,>) || def == typeof(INotificationHandler<>))
				{
					services.AddScoped(itf, type);
				}
			}
		}
	}
}
