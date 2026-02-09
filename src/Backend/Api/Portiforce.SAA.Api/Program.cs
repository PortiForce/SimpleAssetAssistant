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
using Scalar.AspNetCore;

namespace Portiforce.SAA.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// an ability to use user secrets for local runs
		if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Local"))
		{
			builder.Configuration.AddUserSecrets<Program>(optional: true);
		}

		builder.Services.AddControllers();

		// RFC 7807 ProblemDetails support
		builder.Services.AddProblemDetails(options =>
		{
			// attach trace id for correlation
			options.CustomizeProblemDetails = ctx =>
			{
				ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
			};
		});

		// Central exception handling via IExceptionHandler
		builder.Services.AddExceptionHandler<ApiExceptionHandler>();

		builder.Services.AddOpenApi();
		
		//  current user / correlation / multi-tenancy access outside controllers
		builder.Services.AddHttpContextAccessor();

		// Bind + validate JWT settings early (fail fast)
		builder.Services
			.AddOptions<JwtSettings>()
			.Bind(builder.Configuration.GetSection("JwtSettings"))
			.Validate(s =>
					!string.IsNullOrWhiteSpace(s.Issuer) &&
					!string.IsNullOrWhiteSpace(s.Audience) &&
					!string.IsNullOrWhiteSpace(s.Secret) &&
					s.Secret.Length >= 32,
				"JwtSettings are invalid. Issuer/Audience/Secret are required; Secret should be >= 32 chars.")
			.ValidateOnStart();

		builder.Services.AddOptions<TokenHashingOptions>()
			.BindConfiguration("TokenHashingOptions")
			.Validate(o => !string.IsNullOrWhiteSpace(o.Pepper), "TokenHashing:Pepper is required")
			.ValidateOnStart();

		builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
		builder.Services.AddSingleton<IClock, SystemClock>();
		builder.Services.AddSingleton<IHashingService, TokenHashingService>();

		builder.Services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer();

		builder.Services.AddAuthorization();

		// internal dependencies
		RegisterServices(builder);

		// registration of related flows
		builder.Services.AddApplication();
		builder.Services.AddCoreIdentity();
		builder.Services.AddInfrastructure(builder.Configuration);
		builder.Services.AddEfInfrastructure(builder.Configuration);

		var app = builder.Build();

		if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
		{
			app.MapOpenApi();
			app.MapScalarApiReference(options =>
			{
				options.Title = "SimpleAssetAssistant API";
				// If Scalar supports specifying the OpenAPI route in your version, set it explicitly here.
				// Otherwise, it will typically pick up /openapi/v1.json when MapOpenApi() is present.
			});
		}
		else
		{
			app.UseHsts();
		}

		app.UseExceptionHandler();
		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		// run data seeding - do NOT use sa accounts for database flows, only for schema migrations
		if (app.Environment.IsDevelopment())
		{
			// It is safe to run this on every startup in Dev (uncomment when restarting DB model)
			await app.PopulateGlobalDictionariesAndPrepareUserAsync();
		}

		await app.RunAsync();
	}

	private static void RegisterServices(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<ITenantIdServiceResolver, TenantIdServiceResolver>();
	}
}
