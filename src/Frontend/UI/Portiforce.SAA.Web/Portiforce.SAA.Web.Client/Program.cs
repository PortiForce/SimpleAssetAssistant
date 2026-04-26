using System.Globalization;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Web.Client.Configuration;
using Portiforce.SAA.Web.Client.Services;
using Portiforce.SAA.Web.Client.Services.ApiClients;
using Portiforce.SAA.Web.Client.Services.Interfaces;
using Portiforce.SAA.Web.Client.Services.Security;

namespace Portiforce.SAA.Web.Client;

internal class Program
{
	private static async Task Main(string[] args)
	{
		WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

		_ = builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

		_ = builder.Services.AddScoped(sp =>
			new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

		// AuthZ for WASM components (<AuthorizeView>, <AuthorizeRouteView>, [Authorize])
		// should be the same as in server part
		_ = builder.Services.AddAuthorizationCore(options =>
		{
			// Role policies (coarse)
			options.AddPolicy(UiPolicies.PlatformOwner, p => p.RequireRole(UiRoles.PlatformOwner));
			options.AddPolicy(
				UiPolicies.PlatformAdmin,
				p => p.RequireRole(UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
			options.AddPolicy(
				UiPolicies.TenantAdmin,
				p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
			options.AddPolicy(
				UiPolicies.TenantUser,
				p => p.RequireRole(
					UiRoles.TenantUser,
					UiRoles.TenantAdmin,
					UiRoles.PlatformAdmin,
					UiRoles.PlatformOwner));

			// Granular
			options.AddPolicy(
				UiPolicies.InviteUsers,
				p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
		});

		// Authentication state source for <CascadingAuthenticationState>
		_ = builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

		_ = builder.Services.AddScoped<BrowserCredentialsHandler>();
		_ = builder.Services.AddScoped<AntiforgeryTokenStore>();
		_ = builder.Services.AddScoped<AntiforgeryHandler>();

		_ = builder.Services.AddScoped<ITenantUrlContext, TenantUrlContext>();

		_ = builder.Services.AddHttpClient(
				WebClientConstants.NoAntiforgeryClientName,
				client => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); })
			.AddHttpMessageHandler<BrowserCredentialsHandler>();

		_ = builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			})
			.AddHttpMessageHandler<BrowserCredentialsHandler>()
			.AddHttpMessageHandler<AntiforgeryHandler>();

		_ = builder.Services.AddHttpClient<IManageInviteApiClient, ManageInviteApiClient>(client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			})
			.AddHttpMessageHandler<BrowserCredentialsHandler>()
			.AddHttpMessageHandler<AntiforgeryHandler>();

		_ = builder.Services.AddHttpClient<ITenantApiClient, TenantApiClient>(client =>
			{
				client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			})
			.AddHttpMessageHandler<BrowserCredentialsHandler>();

		WebAssemblyHost host = builder.Build();

		IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
		string browserCulture = await js.InvokeAsync<string>("portiforce.getBrowserCulture");

		string cultureMame = NormalizeCulture(browserCulture);
		CultureInfo culture = new(cultureMame);

		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;

		await host.RunAsync();
	}

	private static string NormalizeCulture(string? browserCulture)
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
