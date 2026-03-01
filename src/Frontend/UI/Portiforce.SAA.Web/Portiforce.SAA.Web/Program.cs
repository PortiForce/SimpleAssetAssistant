using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Portiforce.SAA.Application;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Services;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Infrastructure;
using Portiforce.SAA.Infrastructure.EF;
using Portiforce.SAA.Web.Components;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Middleware;
using Portiforce.SAA.Web.Security;
using Portiforce.SAA.Web.Services;

namespace Portiforce.SAA.Web;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		if (builder.Environment.IsDevelopment())
		{
			builder.Configuration.AddUserSecrets<Program>(optional: true);
		}

		// Tenancy
		builder.Services.Configure<TenancyOptions>(builder.Configuration.GetSection(TenancyOptions.SectionName));
		builder.Services.AddScoped<TenantContext>();
		builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
		builder.Services.AddHttpClient<Portiforce.SAA.Web.Client.Services.TenantApiClient>((sp, http) =>
		{
			var ctx = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
			          ?? throw new InvalidOperationException("No active HttpContext.");

			http.BaseAddress = new Uri($"{ctx.Request.Scheme}://{ctx.Request.Host}");
		});

		builder.Services.AddHttpContextAccessor();
		builder.Services.AddMemoryCache();

		builder.Services.AddScoped<ITenantResolver, TenantResolver>();

		// Scan and Register all Endpoints
		builder.Services.AddEndpoints(typeof(Program).Assembly);

		builder.Services.AddAuthentication(options =>
			{
				// This tells ASP.NET that your main way of tracking users is via Cookies
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.LoginPath = "/auth/login";
				options.AccessDeniedPath = "/auth/access-denied";

				options.Events.OnRedirectToLogin = ctx =>
				{
					// For APIs: return 401, for browser navigation: redirect
					if (ctx.Request.Path.StartsWithSegments("/api"))
					{
						ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
						return Task.CompletedTask;
					}
					ctx.Response.Redirect(ctx.RedirectUri);
					return Task.CompletedTask;
				};

				options.Events.OnRedirectToAccessDenied = ctx =>
				{
					if (ctx.Request.Path.StartsWithSegments("/api"))
					{
						ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
						return Task.CompletedTask;
					}
					ctx.Response.Redirect(ctx.RedirectUri);
					return Task.CompletedTask;
				};
			}).AddGoogle(options =>
			{
				// ASP.NET Core automatically maps the scheme name to "Google" here

				options.ClientId = builder.Configuration["GoogleClientSettings:ClientId"]
				                   ?? throw new InvalidOperationException("Google ClientId is missing.");

				options.ClientSecret = builder.Configuration["GoogleClientSettings:ClientSecret"]
				                       ?? throw new InvalidOperationException("Google ClientSecret is missing.");

				// By default, Google will redirect back to: https://app.dev.localhost:7100/signin-google
				// ASP.NET Core automatically intercepts this route for you.
			});

		builder.Services.AddAuthorization(options =>
		{
			// Role policies (coarse)
			options.AddPolicy(UiPolicies.PlatformOwner, p => p.RequireRole(UiRoles.PlatformOwner));
			options.AddPolicy(UiPolicies.PlatformAdmin, p => p.RequireRole(UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
			options.AddPolicy(UiPolicies.TenantAdmin, p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
			options.AddPolicy(UiPolicies.TenantUser, p => p.RequireRole(UiRoles.TenantUser, UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));

			// Granular (start small)
			options.AddPolicy(UiPolicies.InviteUsers, p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
		});

		// -----------------------------
		// Blazor Web App
		// -----------------------------
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents()
			.AddInteractiveWebAssemblyComponents();

		builder.Services.AddApplication();
		builder.Services.AddCoreIdentity();
		builder.Services.AddInfrastructure(builder.Configuration);
		builder.Services.AddEfInfrastructure(builder.Configuration);

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseAntiforgery();

		// Tenant must run early (before auth decisions if you do tenant-bound checks)
		app.UseMiddleware<TenantResolutionMiddleware>();

		// Auth must be before endpoints
		app.UseAuthentication();
		app.UseAuthorization();

		// This line automatically discovers InviteEndpoints, AuthEndpoints, etc.
		app.MapEndpoints();

		//
		// Blazor endpoints
		//
		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(Portiforce.SAA.Web.Client._Imports).Assembly);

		app.Run();
	}
}
