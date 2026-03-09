using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

using Portiforce.SAA.Application;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Services;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Infrastructure;
using Portiforce.SAA.Infrastructure.EF;
using Portiforce.SAA.Infrastructure.Services.Time;
using Portiforce.SAA.Web.Client.Services;
using Portiforce.SAA.Web.Client.Services.Interfaces;
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

		builder.Services.AddHttpContextAccessor();

		// todo alex: check this setup here (should it be rewritten/optimized)?
		builder.Services.AddHttpClient<TenantApiClient>((sp, http) =>
		{
			HttpContext httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
			                          ?? throw new InvalidOperationException("TenantApiClient requires an active HttpContext.");

			http.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
		});

		builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>((sp, http) =>
		{
			HttpContext httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
			                          ?? throw new InvalidOperationException("AdminApiClient requires an active HttpContext.");

			http.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
		});
		
		builder.Services.AddMemoryCache();

		builder.Services.AddScoped<ITenantResolver, TenantResolver>();

		// Scan and Register all Endpoints
		builder.Services.AddEndpoints(typeof(Program).Assembly);

		builder.Services.AddAuthentication(options =>
			{
				// main way of tracking users is via Cookies
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.LoginPath = "/auth/login";
				options.AccessDeniedPath = "/auth/access-denied";

				options.Events.OnRedirectToLogin = ctx =>
				{
					// For APIs/ BFF: return 401, for browser navigation: redirect
					if (ctx.Request.Path.StartsWithSegments("/api") || ctx.Request.Path.StartsWithSegments("/bff"))
					{
						ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
						return Task.CompletedTask;
					}
					ctx.Response.Redirect(ctx.RedirectUri);
					return Task.CompletedTask;
				};

				options.Events.OnRedirectToAccessDenied = ctx =>
				{
					if (ctx.Request.Path.StartsWithSegments("/api") || ctx.Request.Path.StartsWithSegments("/bff"))
					{
						ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
						return Task.CompletedTask;
					}
					ctx.Response.Redirect(ctx.RedirectUri);
					return Task.CompletedTask;
				};
			}).AddGoogle(options =>
			{
				options.ClientId = builder.Configuration["GoogleClientSettings:ClientId"]
								   ?? throw new InvalidOperationException("Google ClientId is missing.");

				options.ClientSecret = builder.Configuration["GoogleClientSettings:ClientSecret"]
									   ?? throw new InvalidOperationException("Google ClientSecret is missing.");

				// Explicitly set the callback path (where Google redirects)
				options.CallbackPath = "/signin-google";

				// Save tokens for later use
				options.SaveTokens = true;

				// Request additional scopes
				options.Scope.Add("profile");
				options.Scope.Add("email");

				// Store correlation ID to preserve state through OAuth flow
				options.UsePkce = true;

				// Handle authentication failures
				options.Events.OnRemoteFailure = context =>
				{
					context.Response.Redirect("/auth/access-denied?error=google_failed");
					context.HandleResponse();
					return Task.CompletedTask;
				};
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

		builder.Services.AddSingleton<IClock, SystemClock>();

		builder.Services.AddAntiforgery(options =>
		{
			options.HeaderName = "RequestVerificationToken";
		});

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

		// figure out endpoints before auth, so we can skip auth for static files, etc.
		app.UseRouting();

		// Tenant must run early (before auth decisions if you do tenant-bound checks)
		app.UseMiddleware<TenantResolutionMiddleware>();

		// Auth must be before endpoints
		app.UseAuthentication();
		app.UseAuthorization();

		// allow to the flow define`.DisableAntiforgery()` metadata on your endpoint!
		app.UseAntiforgery();

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
