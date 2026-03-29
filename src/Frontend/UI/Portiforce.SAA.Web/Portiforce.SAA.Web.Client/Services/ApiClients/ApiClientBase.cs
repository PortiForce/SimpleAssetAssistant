using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;

using Portiforce.SAA.Web.Client.Models;

namespace Portiforce.SAA.Web.Client.Services.ApiClients;

public abstract class ApiClientBase(HttpClient httpClient)
{
	protected async Task<TResponse> GetAsync<TResponse>(
		string url,
		CancellationToken ct = default)
	{
		using HttpResponseMessage response = await httpClient.GetAsync(url, ct);

		if (!response.IsSuccessStatusCode)
		{
			throw await CreateFriendlyExceptionAsync(response, ct);
		}

		return await response.Content.ReadFromJsonAsync<TResponse>(ct)
			   ?? throw new InvalidOperationException("Failed to read the server response.");
	}

	protected async Task<TResponse> PostJsonAsync<TRequest, TResponse>(
		string url,
		TRequest request,
		CancellationToken ct = default)
	{
		using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, request, ct);

		if (!response.IsSuccessStatusCode)
		{
			throw await CreateFriendlyExceptionAsync(response, ct);
		}

		return await response.Content.ReadFromJsonAsync<TResponse>(ct)
			   ?? throw new InvalidOperationException("Failed to read the server response.");
	}

	protected async Task PostAsync(
		string url,
		CancellationToken ct = default)
	{
		using HttpResponseMessage response = await httpClient.PostAsync(url, null, ct);

		if (!response.IsSuccessStatusCode)
		{
			throw await CreateFriendlyExceptionAsync(response, ct);
		}
	}

	protected async Task<TResponse> PostAsync<TResponse>(
		string url,
		CancellationToken ct = default)
	{
		using HttpResponseMessage response = await httpClient.PostAsync(url, null, ct);

		if (!response.IsSuccessStatusCode)
		{
			throw await CreateFriendlyExceptionAsync(response, ct);
		}

		return await response.Content.ReadFromJsonAsync<TResponse>(ct)
			   ?? throw new InvalidOperationException("Failed to read the server response.");
	}

	protected static string BuildUrl(string path) => path;

	protected static string BuildUrl(
		string basePath,
		IEnumerable<KeyValuePair<string, string>> query) =>
		QueryHelpers.AddQueryString(basePath, query);

	private static async Task<Exception> CreateFriendlyExceptionAsync(
		HttpResponseMessage response,
		CancellationToken ct)
	{
		string errorMessage = "An unexpected error occurred.";

		try
		{
			ApiProblemDetails? problem = await response.Content
				.ReadFromJsonAsync<ApiProblemDetails>(ct);

			if (problem?.Errors is not null && problem.Errors.Count > 0)
			{
				errorMessage = problem.Errors.First().Value.FirstOrDefault() ?? errorMessage;
			}
			else
			{
				errorMessage = problem?.Detail ?? problem?.Title ?? errorMessage;
			}
		}
		catch
		{
			// Fall back to generic message.
		}

		return new ApplicationException(errorMessage);
	}
}