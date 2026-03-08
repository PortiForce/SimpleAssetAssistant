using System.Net.Http.Json;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Invite;
using Portiforce.SAA.Web.Client.Models;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class AdminApiClient(
	HttpClient httpClient) : IAdminApiClient
{
	public async Task<CreateInviteResponse> InviteUserAsync(
		CreateInviteRequest request,
		CancellationToken ct = default)
	{
		using var response = await httpClient.PostAsJsonAsync(ApiRoutes.Admin, request, ct);

		if (!response.IsSuccessStatusCode)
		{
			throw await CreateFriendlyExceptionAsync(response, ct);
		}

		return await response.Content.ReadFromJsonAsync<CreateInviteResponse>(cancellationToken: ct)
		       ?? throw new InvalidOperationException("Failed to read the server response.");
	}

	private static async Task<Exception> CreateFriendlyExceptionAsync(
		HttpResponseMessage response,
		CancellationToken ct)
	{
		string errorMessage = "An unexpected error occurred while sending the invite.";

		try
		{
			ApiProblemDetails? problem = await response.Content
				.ReadFromJsonAsync<ApiProblemDetails>(cancellationToken: ct);

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
			// Ignore deserialization issues and fall back to generic message.
		}

		return new ApplicationException(errorMessage);
	}
}