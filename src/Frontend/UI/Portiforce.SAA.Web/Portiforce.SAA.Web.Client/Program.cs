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

		IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
		var browserCulture = await js.InvokeAsync<string>("portiforce.getBrowserCulture");

		string cultureMame = NormalizeCulture(browserCulture);
		var culture = new CultureInfo(cultureMame);

		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;

		await host.RunAsync();
	}

	static string NormalizeCulture(string? browserCulture)
	{
		if (string.IsNullOrWhiteSpace(browserCulture))
		{
			return "en-US";
		}

		browserCulture = browserCulture.Trim();

		if (browserCulture.StartsWith("uk", StringComparison.OrdinalIgnoreCase))
		{
			return "uk-UA";
		}

		if (browserCulture.StartsWith("en", StringComparison.OrdinalIgnoreCase))
		{
			return "en-US";
		}

		// fallback to en-US if unsupported culture is detected
		return "en-US";
	}
}
