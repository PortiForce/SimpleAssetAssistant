using Portiforce.SAA.Web.Client.Configuration;

namespace Portiforce.SAA.Web.Client.Services.Security;

/// <summary>
/// todo : tech review
/// </summary>
/// <param name="tokenStore"></param>
public sealed class AntiforgeryHandler(AntiforgeryTokenStore tokenStore) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		if (request.Method != HttpMethod.Get &&
		    request.Method != HttpMethod.Head &&
		    request.Method != HttpMethod.Options &&
		    request.Method != HttpMethod.Trace)
		{
			var token = await tokenStore.GetAsync(ct);
			if (!string.IsNullOrWhiteSpace(token))
			{
				request.Headers.TryAddWithoutValidation(WebClientConstants.ReqVerificationTokenName, token);
			}
		}

		return await base.SendAsync(request, ct);
	}
}