using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;
using Microsoft.JSInterop;

using Portiforce.SAA.Contracts.UiSetup;
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

		builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

		builder.Services.AddScoped(sp => new HttpClient
		{
			BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
		});

		// AuthZ for WASM components (<AuthorizeView>, <AuthorizeRouteView>, [Authorize])
		// should be the same as in server part
		builder.Services.AddAuthorizationCore(options =>
		{
			// Role policies (coarse)
			options.AddPolicy(UiPolicies.PlatformOwner, p => p.RequireRole(UiRoles.PlatformOwner));
			options.AddPolicy(UiPolicies.PlatformAdmin, p => p.RequireRole(UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
			options.AddPolicy(UiPolicies.TenantAdmin, p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
			options.AddPolicy(UiPolicies.TenantUser, p => p.RequireRole(UiRoles.TenantUser, UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));

			// Granular
			options.AddPolicy(UiPolicies.InviteUsers, p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
		});

		// Authentication state source for <CascadingAuthenticationState>
		builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

		builder.Services.AddScoped<BrowserCredentialsHandler>();
		builder.Services.AddScoped<AntiforgeryTokenStore>();
		builder.Services.AddScoped<AntiforgeryHandler>();
			
		builder.Services.AddHttpClient(WebClientConstants.NoAntiforgeryClientName, client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			})
			.AddHttpMessageHandler<BrowserCredentialsHandler>();

		builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			})
			.AddHttpMessageHandler<BrowserCredentialsHandler>()
			.AddHttpMessageHandler<AntiforgeryHandler>();

		builder.Services.AddScoped<TenantApiClient>();

		var host = builder.Build();

		var js = host.Services.GetRequiredService<IJSRuntime>();
		var cultureName = await js.InvokeAsync<string>("portiforce.getBrowserCulture");
		var culture = CultureInfo.GetCultureInfo(cultureName);

		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;

		await host.RunAsync();
	}
}
