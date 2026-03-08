using System.Net.Http.Json;

using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.Models.Client.User;
using Portiforce.SAA.Web.Client.Models;
using Portiforce.SAA.Web.Client.Services.Interfaces;
using GetInviteDetailsRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteDetailsRequest;
using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class AdminApiClient(
	HttpClient httpClient) : IAdminApiClient
{
	public async Task<InviteListResponse> GetInvitesAsync(GetInviteListQueryRequest request, CancellationToken ct = default)
	{
		// Minimal query string builder for pagination
		var url = $"bff/invites?page={request.Page}&pageSize={request.PageSize}";

		// You can add search/status/channel to the query string here later as needed

		var response = await httpClient.GetFromJsonAsync<InviteListResponse>(url, ct);

		return response ?? throw new InvalidOperationException("Invites: failed to read the server response.");
	}

	public async Task<InviteDetailsResponse> GetInviteDetailsAsync(GetInviteDetailsRequest request, CancellationToken ct = default)
	{
		var url = $"bff/invites?inviteId={request.InviteId}";

		var response = await httpClient.GetFromJsonAsync<InviteDetailsResponse>(url, ct);

		return response ?? throw new InvalidOperationException("Invite details, failed to read the server response");
	}

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

	public async Task<AccountListResponse> GetUsersAsync(GetAccountListQueryRequest request, CancellationToken ct = default)
	{
		// Minimal query string builder for pagination
		var url = $"bff/accounts?page={request.Page}&pageSize={request.PageSize}";

		// You can add search/status/channel to the query string here later as needed

		var response = await httpClient.GetFromJsonAsync<AccountListResponse>(url, ct);

		return response ?? throw new InvalidOperationException("Users: failed to read the server response.");
	}

	public async Task<AccountDetailsResponse> GetUserDetailsAsync(GetAccountDetailsRequest request, CancellationToken ct = default)
	{
		var url = $"bff/accounts?accountId={request.AccountId}";

		var response = await httpClient.GetFromJsonAsync<AccountDetailsResponse>(url, ct);

		return response ?? throw new InvalidOperationException("User details, failed to read the server response");
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