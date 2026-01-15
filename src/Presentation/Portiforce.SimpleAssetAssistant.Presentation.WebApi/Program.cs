
using Microsoft.AspNetCore.Mvc.Infrastructure;

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

		// Add services to the container.

		builder.Services.AddControllers();
		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi();

		builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}
