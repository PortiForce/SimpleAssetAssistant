using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

using Portiforce.SAA.Api.Configuration;
using Portiforce.SAA.Api.ErrorHandling;
using Portiforce.SAA.Api.Interfaces;
using Portiforce.SAA.Api.Services;
using Portiforce.SAA.Application;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Infrastructure;
using Portiforce.SAA.Infrastructure.Configuration;
using Portiforce.SAA.Infrastructure.EF;
using Portiforce.SAA.Infrastructure.EF.DataPopulation;
using Portiforce.SAA.Infrastructure.Services.Security;
using Portiforce.SAA.Infrastructure.Services.Time;
using Portiforce.SAA.Notifications.Worker;

using Scalar.AspNetCore;

namespace Portiforce.SAA.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		// an ability to use user secrets for local runs
		if (builder.Environment.IsDevelopment())
		{
			_ = builder.Configuration.AddUserSecrets<Program>(true);
		}

		_ = builder.Services.AddControllers();

		// RFC 7807 ProblemDetails support
		_ = builder.Services.AddProblemDetails(options =>
		{
			// attach trace id for correlation
			options.CustomizeProblemDetails = ctx =>
			{
				ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
			};
		});

		// Central exception handling via IExceptionHandler
		_ = builder.Services.AddExceptionHandler<ApiExceptionHandler>();

		_ = builder.Services.AddOpenApi();

		// use case: current user / correlation / multi-tenancy access outside controllers
		_ = builder.Services.AddHttpContextAccessor();

		// Bind + validate JWT settings early (fail fast)
		_ = builder.Services
			.AddOptions<JwtSettings>()
			.Bind(builder.Configuration.GetSection("JwtSettings"))
			.Validate(
				s =>
					!string.IsNullOrWhiteSpace(s.Issuer) &&
					!string.IsNullOrWhiteSpace(s.Audience) &&
					!string.IsNullOrWhiteSpace(s.Secret) &&
					s.Secret.Length >= 32,
				"JwtSettings are invalid. Issuer/Audience/Secret are required; Secret should be >= 32 chars.")
			.ValidateOnStart();

		_ = builder.Services.AddOptions<TokenHashingOptions>()
			.BindConfiguration("TokenHashingOptions")
			.Validate(o => !string.IsNullOrWhiteSpace(o.Pepper), "TokenHashing:Pepper is required")
			.ValidateOnStart();

		_ = builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
		_ = builder.Services.AddSingleton<IClock, SystemClock>();
		_ = builder.Services.AddSingleton<IHashingService, TokenHashingService>();

		_ = builder.Services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer();

		_ = builder.Services.AddAuthorization();

		// internal dependencies
		RegisterServices(builder);

		// registration of related flows
		_ = builder.Services.AddApplication();
		_ = builder.Services.AddCoreIdentity();
		_ = builder.Services.AddInfrastructure(builder.Configuration);
		_ = builder.Services.AddEfInfrastructure(builder.Configuration);
		_ = builder.Services.AddNotificationsInfrastructure(builder.Configuration);

		WebApplication app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			_ = app.MapOpenApi();
			_ = app.MapScalarApiReference(options => { options.Title = "SimpleAssetAssistant API"; });
		}
		else
		{
			_ = app.UseHsts();
		}

		_ = app.UseExceptionHandler();
		_ = app.UseHttpsRedirection();

		_ = app.UseAuthentication();
		_ = app.UseAuthorization();

		_ = app.MapControllers();

		// run data seeding - do NOT use sa accounts for database flows, only for schema migrations
		if (app.Environment.IsDevelopment())
		{
			// It is safe to run this on every startup in Dev (uncomment when restarting DB model)
			await app.PopulateGlobalDictionariesAndPrepareUserAsync();
		}

		await app.RunAsync();
	}

	private static void RegisterServices(WebApplicationBuilder builder) =>
		_ = builder.Services.AddScoped<ITenantIdServiceResolver, TenantIdServiceResolver>();
}