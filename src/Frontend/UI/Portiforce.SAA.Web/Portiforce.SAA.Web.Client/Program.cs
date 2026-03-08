using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portiforce.SAA.Web.Client.Configuration;
using Portiforce.SAA.Web.Client.Services;
using Portiforce.SAA.Web.Client.Services.Interfaces;
using Portiforce.SAA.Web.Client.Services.Security;

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

		// A named client without antiforgery handler (used to fetch the token)
		builder.Services.AddHttpClient(WebClientConstants.NoAntiforgeryClientName, client =>
		{
			client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
		});

		builder.Services.AddScoped<AntiforgeryTokenStore>();
		builder.Services.AddScoped<AntiforgeryHandler>();

		// Typed API client (BFF) with antiforgery header automatically attached
		builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
		{
			client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
		}).AddHttpMessageHandler<AntiforgeryHandler>();

		builder.Services.AddScoped<TenantApiClient>();

		await builder.Build().RunAsync();
	}
}
