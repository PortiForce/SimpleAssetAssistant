using System.Net.Http.Json;

using Portiforce.SAA.Contracts.Models.Invite;
using Portiforce.SAA.Web.Client.Models;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class AdminApiClient(HttpClient httpClient) : IAdminApiClient
{
	public async Task<CreateInviteResponse> InviteUserAsync(CreateInviteRequest request, CancellationToken ct = default)
	{
		// 1. Corrected the URL to match the endpoint
		var response = await httpClient.PostAsJsonAsync("api/v1/tenant/invites", request, ct);

		// 2. Handle Errors Gracefully
		if (!response.IsSuccessStatusCode)
		{
			string errorMessage = "An unexpected error occurred while sending the invite.";

			try
			{
				// Attempt to read standard RFC 7807 ProblemDetails returned by Minimal APIs
				var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(cancellationToken: ct);

				if (problem.Errors is not null && problem.Errors.Any())
				{
					// check if it's a Validation Problem (400)
					errorMessage = problem.Errors.First().Value.FirstOrDefault() ?? errorMessage;
				}
				else
				{
					// check for a specific business logic detail or title
					errorMessage = problem.Detail ?? problem.Title ?? errorMessage;
				}
			}
			catch
			{
				// If it wasn't JSON (e.g., a 502 Bad Gateway from a proxy), fallback to standard exception
				response.EnsureSuccessStatusCode();
			}

			// Throw a clean exception with the exact backend message
			throw new ApplicationException(errorMessage);
		}

		// 3. Deserialize and return the successful response body
		var result = await response.Content.ReadFromJsonAsync<CreateInviteResponse>(cancellationToken: ct);

		return result ?? throw new InvalidOperationException("Failed to read the server response.");
	}
}
