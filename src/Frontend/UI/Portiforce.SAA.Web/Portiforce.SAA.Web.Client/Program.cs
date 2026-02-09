using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portiforce.SAA.Web.Client.Services;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client;

internal class Program
{
	static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);

		builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
		{
			// Base address is the current origin (The BFF)
			client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
		});

		await builder.Build().RunAsync();
	}
}
