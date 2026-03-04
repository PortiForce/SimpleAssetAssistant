using System.Net.Http.Json;

using Portiforce.SAA.Contracts.Models.Invite;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class AdminApiClient(HttpClient httpClient) : IAdminApiClient
{
	public async Task InviteUserAsync(CreateInviteRequest request)
	{
		var response = await httpClient.PostAsJsonAsync("api/v1/tenant/users", request);
		response.EnsureSuccessStatusCode();
	}
}
