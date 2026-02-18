using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Services;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Infrastructure.EF;
using Portiforce.SAA.Web.Components;
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
		builder.Services.AddScoped<Portiforce.SAA.Web.Client.Services.TenantApiClient>(sp =>
		{
			var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
							  ?? throw new InvalidOperationException("No active HttpContext.");

			var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
			var http = new HttpClient { BaseAddress = baseUri };

			return new Portiforce.SAA.Web.Client.Services.TenantApiClient(http);
		});

		builder.Services.AddHttpContextAccessor();
		builder.Services.AddMemoryCache();

		builder.Services.AddEfInfrastructure(builder.Configuration);

		builder.Services.AddScoped<ITenantResolver, TenantResolver>();

		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.LoginPath = "/auth/login";
				options.LogoutPath = "/auth/logout";
				options.AccessDeniedPath = "/access-denied";
				options.SlidingExpiration = true;
				options.ExpireTimeSpan = TimeSpan.FromHours(8);

				// Optional hardening:
				// options.Cookie.SameSite = SameSiteMode.Lax;
				// options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

		// 
		// Basic endpoints for now (avoid 404 while wiring auth)
		//
		app.MapGet("/access-denied", () => Results.Text("Access denied."));

		// Placeholder until you wire Google/Passkeys
		app.MapGet("/auth/login", () => Results.Text("Login not wired yet."));

		app.MapPost("/auth/logout", async (HttpContext ctx) =>
		{
			await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Results.Redirect("/");
		});

		//
		// UI API (BFF endpoints)
		// 
		var uiApi = app.MapGroup("/ui-api");

		uiApi.MapGet("/tenant", (ITenantContext tenant) =>
		{
			return Results.Ok(new
			{
				prefix = tenant.Prefix,
				isLanding = tenant.IsLanding
			});
		});

		// Example admin endpoint (add later)
		// uiApi.MapPost("/admin/users/invite", ...).RequireAuthorization(Policies.InviteUsers)

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
