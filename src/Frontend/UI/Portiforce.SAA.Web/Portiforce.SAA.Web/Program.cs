using System.Globalization;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;

using Portiforce.SAA.Application;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Services;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Infrastructure;
using Portiforce.SAA.Infrastructure.Configuration;
using Portiforce.SAA.Infrastructure.EF;
using Portiforce.SAA.Infrastructure.Services.Time;
using Portiforce.SAA.Web.Client.Services;
using Portiforce.SAA.Web.Client.Services.ApiClients;
using Portiforce.SAA.Web.Client.Services.Interfaces;
using Portiforce.SAA.Web.Components;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Middleware;
using Portiforce.SAA.Web.Security;
using Portiforce.SAA.Web.Services;

using _Imports = Portiforce.SAA.Web.Client._Imports;

namespace Portiforce.SAA.Web;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		_ = builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

		if (builder.Environment.IsDevelopment())
		{
			_ = builder.Configuration.AddUserSecrets<Program>(true);
		}

		// Tenancy
		_ = builder.Services.AddOptions<TenancyOptions>()
			.Bind(builder.Configuration.GetSection(TenancyOptions.SectionName))
			.Validate(
				o => IsConfiguredValue(o.BaseDomain),
				"Tenancy:BaseDomain must be configured.")
			.ValidateOnStart();
		_ = builder.Services.AddScoped<TenantContext>();
		_ = builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

		_ = builder.Services.AddHttpContextAccessor();

		// todo alex: check this setup here (should it be rewritten/optimized)?
		_ = builder.Services.AddHttpClient<TenantApiClient>((sp, http) =>
		{
			HttpContext httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
									  ?? throw new InvalidOperationException(
										  "TenantApiClient requires an active HttpContext.");

			http.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
		});

		_ = builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>((sp, http) =>
		{
			HttpContext httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
									  ?? throw new InvalidOperationException(
										  "AdminApiClient requires an active HttpContext.");

			http.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
		});

		_ = builder.Services.AddHttpClient<IManageInviteApiClient, ManageInviteApiClient>((sp, http) =>
		{
			HttpContext httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
									  ?? throw new InvalidOperationException(
										  "ManageInvitesApiClient requires an active HttpContext.");

			http.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
		});

		_ = builder.Services.AddMemoryCache();

		_ = builder.Services.AddScoped<ITenantResolver, TenantResolver>();
		_ = builder.Services.AddScoped<ITenantUrlContext, TenantUrlContext>();

		// Scan and Register all Endpoints
		_ = builder.Services.AddEndpoints(typeof(Program).Assembly);

		_ = builder.Services.AddAuthentication(options =>
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
				options.Cookie.HttpOnly = true;
				options.Cookie.SameSite = SameSiteMode.Lax;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				options.ExpireTimeSpan = TimeSpan.FromHours(8);
				options.SlidingExpiration = false;

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
				string? clientId = builder.Configuration["GoogleClientSettings:ClientId"];
				if (!IsConfiguredValue(clientId))
				{
					throw new InvalidOperationException("Google ClientId is missing.");
				}

				string? clientSecret = builder.Configuration["GoogleClientSettings:ClientSecret"];
				if (!IsConfiguredValue(clientSecret))
				{
					throw new InvalidOperationException("Google ClientSecret is missing.");
				}

				options.ClientId = clientId;
				options.ClientSecret = clientSecret;

				// Explicitly set the callback path (where Google redirects)
				options.CallbackPath = "/signin-google";

				options.SaveTokens = false;

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

		_ = builder.Services.AddAuthorization(options =>
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

			// Granular (start small)
			options.AddPolicy(
				UiPolicies.InviteUsers,
				p => p.RequireRole(UiRoles.TenantAdmin, UiRoles.PlatformAdmin, UiRoles.PlatformOwner));
		});

		// -----------------------------
		// Blazor Web App
		// -----------------------------
		_ = builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents()
			.AddInteractiveWebAssemblyComponents();

		_ = builder.Services.AddApplication();
		_ = builder.Services.AddCoreIdentity();
		_ = builder.Services.AddInfrastructure(builder.Configuration);
		_ = builder.Services.AddEfInfrastructure(builder.Configuration);

		_ = builder.Services.AddOptions<TokenHashingOptions>()
			.BindConfiguration("TokenHashingOptions")
			.Validate(o => IsConfiguredValue(o.Pepper), "TokenHashingOptions:Pepper must be configured.")
			.ValidateOnStart();

		_ = builder.Services.AddSingleton<IClock, SystemClock>();

		_ = builder.Services.AddAntiforgery(options => { options.HeaderName = "RequestVerificationToken"; });

		_ = builder.Services.AddProblemDetails();

		_ = builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

		WebApplication app = builder.Build();

		_ = app.Use(async (context, next) =>
		{
			context.Response.Headers["Content-Security-Policy"] = BuildContentSecurityPolicy(app.Environment);
			await next(context);
		});

		CultureInfo[] supportedCultures = new[]
		{
			new CultureInfo("en"), new CultureInfo("en-US"), new CultureInfo("uk"), new CultureInfo("uk-UA")
		};

		RequestLocalizationOptions localizationOptions = new()
		{
			DefaultRequestCulture = new RequestCulture("en-US"),
			SupportedCultures = supportedCultures,
			SupportedUICultures = supportedCultures,
			RequestCultureProviders =
			[
				new AcceptLanguageHeaderRequestCultureProvider()
			]
		};

		_ = app.UseRequestLocalization(localizationOptions);

		if (app.Environment.IsDevelopment())
		{
			app.UseWebAssemblyDebugging();
		}
		else
		{
			_ = app.UseHsts();
		}

		_ = app.UseExceptionHandler();
		_ = app.UseHttpsRedirection();
		_ = app.UseStaticFiles();
		_ = app.UseRouting();

		// Tenant must run early
		_ = app.UseMiddleware<TenantResolutionMiddleware>();

		// Auth must be before endpoints
		_ = app.UseAuthentication();
		_ = app.UseAuthorization();

		// allow to the flow define`.DisableAntiforgery()` metadata on your endpoint!
		_ = app.UseAntiforgery();

		// This line automatically discovers InviteEndpoints, AuthEndpoints, etc.
		_ = app.MapEndpoints();

		//
		// Blazor endpoints
		//
		_ = app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(_Imports).Assembly);

		app.Run();
	}

	private static bool IsConfiguredValue(string? value) =>
		!string.IsNullOrWhiteSpace(value) &&
		!value.Contains("{from-configs}", StringComparison.OrdinalIgnoreCase);

	private static string BuildContentSecurityPolicy(IHostEnvironment environment)
	{
		string scriptEvalSource = environment.IsDevelopment() ? "'unsafe-eval'" : "'wasm-unsafe-eval'";

		return string.Join(
			" ",
			"default-src 'self';",
			$"script-src 'self' 'unsafe-inline' {scriptEvalSource} https://accounts.google.com https://apis.google.com;",
			"style-src 'self' 'unsafe-inline' https://accounts.google.com;",
			"img-src 'self' data: https:;",
			"font-src 'self' data:;",
			"connect-src 'self' wss: https://accounts.google.com;",
			"frame-src 'self' https://accounts.google.com;",
			"frame-ancestors 'self';",
			"base-uri 'self';",
			"form-action 'self';",
			"object-src 'none';");
	}
}