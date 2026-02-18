using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Portiforce.SAA.Web.Client.Services;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client;

internal class Program
{
	static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);

		// Default HttpClient for simple services like TenantApiClient
		builder.Services.AddScoped(sp => new HttpClient
		{
			BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
		});

		// Typed API client
		builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
		{
			// Base address is the current origin (The BFF)
			client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
		});

		builder.Services.AddScoped<TenantApiClient>();

		await builder.Build().RunAsync();
	}
}
