using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Account;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Web.Client.Services.ApiClients;
using Portiforce.SAA.Web.Client.Services.Interfaces;

using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class AdminApiClient(
	HttpClient httpClient) : ApiClientBase(httpClient), IAdminApiClient
{
	public async Task<InviteListResponse> GetInvitesAsync(
		GetInviteListQueryRequest request,
		CancellationToken ct = default)
	{
		string url = BuildUrl(ApiRoutes.Invites, request.ToQueryParameters());
		return await GetAsync<InviteListResponse>(url, ct);
	}

	public async Task<InviteDetailsResponse> GetInviteDetailsAsync(Guid inviteId, CancellationToken ct = default)
	{
		string url = $"{ApiRoutes.Invites}/{inviteId}";
		return await GetAsync<InviteDetailsResponse>(url, ct);
	}

	public async Task<CreateInviteResponse> InviteUserAsync(
		CreateInviteRequest request,
		CancellationToken ct = default)
	{
		return await PostJsonAsync<CreateInviteRequest, CreateInviteResponse>(
			ApiRoutes.Invites,
			request,
			ct);
	}

	public async Task<AccountListResponse> GetUsersAsync(GetAccountListQueryRequest request, CancellationToken ct = default)
	{
		string url = BuildUrl(ApiRoutes.Invites, request.ToQueryParameters());
		return await GetAsync<AccountListResponse>(url, ct);
	}

	public async Task<AccountDetailsResponse> GetUserDetailsAsync(Guid  accountId, CancellationToken ct = default)
	{
		string url = $"{ApiRoutes.Invites}/{accountId}";
		return await GetAsync<AccountDetailsResponse>(url, ct);
	}
}