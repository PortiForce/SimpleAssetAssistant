using Microsoft.Extensions.DependencyInjection;

namespace Portiforce.SAA.Core.Identity;

public static class DependencyInjection
{
	public static IServiceCollection AddCoreIdentity(this IServiceCollection services)
	{
		// todo tech: add dependencies if any
		return services;
	}
}