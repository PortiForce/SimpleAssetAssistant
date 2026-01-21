using Portiforce.SimpleAssetAssistant.Application;
using Portiforce.SimpleAssetAssistant.Core.Identity;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.ErrorHandling;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi;

public class Program
{
	public static void Main(string[] args)
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

				// If you decide to use stable error codes, your ApiExceptionHandler should set:
				// ctx.ProblemDetails.Extensions["code"] = "PF-409-DUPLICATE_EXTERNAL_ID";
				//
				// Keep CustomizeProblemDetails as "global defaults" only,
				// and put exception-specific mapping into ApiExceptionHandler.
			};
		});

		// Central exception handling via IExceptionHandler
		builder.Services.AddExceptionHandler<ApiExceptionHandler>();

		builder.Services.AddOpenApi();

		//  current user / correlation / multi-tenancy access outside controllers
		builder.Services.AddHttpContextAccessor();

		// registration of related flows
		builder.Services.AddApplication();
		builder.Services.AddIdentity();
		builder.Services.AddEfInfrastructure(builder.Configuration);

		var app = builder.Build();

		if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
		{
			app.MapOpenApi();
		}

		app.UseExceptionHandler();

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}
