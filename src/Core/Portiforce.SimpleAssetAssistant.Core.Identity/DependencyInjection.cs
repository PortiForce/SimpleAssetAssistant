using Microsoft.Extensions.DependencyInjection;

namespace Portiforce.SimpleAssetAssistant.Core.Identity;

public static class DependencyInjection
{
	public static IServiceCollection AddIdentity(this IServiceCollection services)
	{
		// todo tech: add dependencies if any
		return services;
	}
}