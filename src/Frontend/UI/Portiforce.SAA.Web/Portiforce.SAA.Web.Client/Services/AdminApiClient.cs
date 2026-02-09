using System.Net.Http.Json;
using Portiforce.SAA.Contracts.User.Requests;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services;

public class AdminApiClient(HttpClient httpClient) : IAdminApiClient
{
	public async Task InviteUserAsync(InviteUserRequest request)
	{
		var response = await httpClient.PostAsJsonAsync("api/v1/tenant/users", request);
		response.EnsureSuccessStatusCode();
	}
}
