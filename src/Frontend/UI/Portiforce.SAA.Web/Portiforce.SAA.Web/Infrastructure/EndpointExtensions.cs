using System.Reflection;

namespace Portiforce.SAA.Web.Infrastructure;

public static class EndpointExtensions
{
	public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
	{
		var endpointTypes = assembly.GetTypes()
			.Where(t =>
				t is { IsClass: true, IsAbstract: false }
				&& t.IsAssignableTo(typeof(IEndpoint)));

		foreach (var type in endpointTypes)
		{
			services.AddTransient(typeof(IEndpoint), type);
		}

		return services;
	}

	public static IApplicationBuilder MapEndpoints(this WebApplication app)
	{
		using IServiceScope scope = app.Services.CreateScope();
		IEnumerable<IEndpoint> endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

		foreach (var endpoint in endpoints)
		{
			endpoint.MapEndpoint(app);
		}

		return app;
	}
}
