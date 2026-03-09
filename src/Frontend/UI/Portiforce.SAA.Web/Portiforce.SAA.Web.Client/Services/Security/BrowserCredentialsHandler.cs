using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Portiforce.SAA.Web.Client.Services.Security;

public sealed class BrowserCredentialsHandler : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		// Required for cookie-based auth when calling the BFF from WebAssembly
		request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

		return base.SendAsync(request, ct);
	}
}
