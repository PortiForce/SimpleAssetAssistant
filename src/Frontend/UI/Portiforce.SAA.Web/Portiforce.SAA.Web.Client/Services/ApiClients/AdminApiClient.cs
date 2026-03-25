using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Account;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.Models.Client.Invite.Summary;
using Portiforce.SAA.Web.Client.Services.ApiClients;
using Portiforce.SAA.Web.Client.Services.Interfaces;

using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class AdminApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IAdminApiClient
{
	public async Task<InviteListResponse> GetInvitesAsync(
		GetInviteListQueryRequest request,
		CancellationToken ct = default)
	{
		string url = BuildUrl(ApiRoutes.Invites.Root, request.ToQueryParameters());
		return await this.GetAsync<InviteListResponse>(url, ct);
	}

	public async Task<InviteSummaryResponse> GetInviteSummaryAsync(
		GetInviteSummaryRequest request,
		CancellationToken ct = default)
	{
		string url = BuildUrl(ApiRoutes.Invites.Summary, request.ToQueryParameters());
		return await this.GetAsync<InviteSummaryResponse>(url, ct);
	}

	public async Task<InviteDetailsResponse> GetInviteDetailsAsync(
		Guid inviteId,
		CancellationToken ct = default)
	{
		string url = $"{ApiRoutes.Invites.Details(inviteId)}";
		return await this.GetAsync<InviteDetailsResponse>(url, ct);
	}

	public async Task<CreateInviteResponse> InviteUserAsync(
		CreateInviteRequest request,
		CancellationToken ct = default)
	{
		return await this.PostJsonAsync<CreateInviteRequest, CreateInviteResponse>(
			ApiRoutes.Invites.New,
			request,
			ct);
	}

	public async Task<bool> RevokeInviteAsync(Guid inviteId, CancellationToken ct = default)
	{
		string url = $"{ApiRoutes.Invites.InviteRevoke(inviteId)}";
		try
		{
			await this.PostAsync(url, ct);
		}
		catch (Exception)
		{
			// Log the exception if needed
			return false;
		}

		return true;
	}

	public async Task<AccountListResponse> GetUsersAsync(
		GetAccountListQueryRequest request,
		CancellationToken ct = default)
	{
		string url = BuildUrl(ApiRoutes.Accounts.Root, request.ToQueryParameters());
		return await this.GetAsync<AccountListResponse>(url, ct);
	}

	public async Task<AccountDetailsResponse> GetUserDetailsAsync(Guid accountId, CancellationToken ct = default)
	{
		string url = $"{ApiRoutes.Accounts.Details(accountId)}";
		return await this.GetAsync<AccountDetailsResponse>(url, ct);
	}
}