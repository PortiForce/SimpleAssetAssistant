using Portiforce.SAA.Web.Components;
using Yarp.ReverseProxy.Transforms;

namespace Portiforce.SAA.Web;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddReverseProxy()
			.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
			.AddTransforms(builderContext =>
			{
				// Extract Tenant from Subdomain and pass as Header
				builderContext.AddRequestTransform(transformContext =>
				{
					var host = transformContext.HttpContext.Request.Host.Host;

					// Configurable base domain
					var baseDomain = "dev.localhost";

					if (host.EndsWith(baseDomain, StringComparison.OrdinalIgnoreCase))
					{
						var prefixLength = host.Length - baseDomain.Length - 1;

						if (prefixLength > 0)
						{
							var prefix = host.Substring(0, prefixLength);
							transformContext.ProxyRequest.Headers.Add("X-Tenant", prefix);
						}
					}

					return ValueTask.CompletedTask;
				});
			});

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

		app.MapReverseProxy();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

		app.Run();
	}
}