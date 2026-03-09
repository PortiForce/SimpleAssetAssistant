using System.Net.Http.Json;
using Portiforce.SAA.Web.Client.Configuration;

namespace Portiforce.SAA.Web.Client.Services.Security;

/// <summary>
/// todo : tech review
/// </summary>
/// <param name="httpClientFactory"></param>
public sealed class AntiforgeryTokenStore(IHttpClientFactory httpClientFactory)
{
	private readonly SemaphoreSlim _gate = new(1, 1);
	private string? _token;

	public async ValueTask<string?> GetAsync(CancellationToken ct)
	{
		if (!string.IsNullOrWhiteSpace(_token))
		{
			return _token;
		}

		await _gate.WaitAsync(ct);
		try
		{
			if (!string.IsNullOrWhiteSpace(_token))
			{
				return _token;
			}

			var http = httpClientFactory.CreateClient(WebClientConstants.NoAntiforgeryClientName);
			var response = await http.GetFromJsonAsync<AntiforgeryTokenResponse>("/bff/antiforgery", ct);

			_token = response?.Token;
			return _token;
		}
		finally
		{
			_gate.Release();
		}
	}

	private sealed record AntiforgeryTokenResponse(string? Token);
}