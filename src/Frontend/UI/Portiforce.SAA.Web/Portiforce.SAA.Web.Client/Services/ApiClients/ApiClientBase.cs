using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.WebUtilities;

using Portiforce.SAA.Contracts.Exceptions;

// todo: does it make sense to have not MS specific stack here?
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
		string defaultMessage = response.ReasonPhrase switch
		{
			{ Length: > 0 } reasonPhrase => reasonPhrase,
			_ => "An unexpected error occurred."
		};

		try
		{
			string content = await response.Content.ReadAsStringAsync(ct);

			if (string.IsNullOrWhiteSpace(content))
			{
				return new HttpRequestException(defaultMessage, null, response.StatusCode);
			}

			ApiValidationProblemDetails? validationProblem = JsonSerializer.Deserialize<ApiValidationProblemDetails>(
				content,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			string? validationMessage =
				validationProblem?.Errors
					.Where(static pair => !string.IsNullOrWhiteSpace(pair.Value))
					.Select(static pair => pair.Value)
					.FirstOrDefault();

			if (!string.IsNullOrWhiteSpace(validationMessage))
			{
				return new HttpRequestException(validationMessage, null, response.StatusCode);
			}

			ApiProblemDetails? problem = JsonSerializer.Deserialize<ApiProblemDetails>(
				content,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			string? message =
				TryGetExtensionString(problem?.Extensions, "message")
				?? problem?.Detail
				?? problem?.Title;

			if (!string.IsNullOrWhiteSpace(message))
			{
				return new HttpRequestException(message, null, response.StatusCode);
			}
		}
		catch
		{
			// Fall back to status/reason phrase below.
		}

		return new HttpRequestException(defaultMessage, null, response.StatusCode);
	}

	private static string? TryGetExtensionString(
		Dictionary<string, object?>? extensions,
		string key)
	{
		if (extensions is null || !extensions.TryGetValue(key, out object? value) || value is null)
		{
			return null;
		}

		return value switch
		{
			string text when !string.IsNullOrWhiteSpace(text) => text,
			JsonElement { ValueKind: JsonValueKind.String } json => json.GetString(),
			JsonElement json => json.ToString(),
			_ => value.ToString()
		};
	}
}