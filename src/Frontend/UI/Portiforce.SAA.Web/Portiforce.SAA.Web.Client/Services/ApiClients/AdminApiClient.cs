using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Exceptions;
using Portiforce.SAA.Contracts.Models.Client.Account;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.Models.Client.Invite.Summary;
using Portiforce.SAA.Web.Client.Services.Interfaces;

using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;

namespace Portiforce.SAA.Web.Client.Services.ApiClients;

public sealed class AdminApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IAdminApiClient
{
	public async Task<InviteListResponse> GetInvitesAsync(
		GetInviteListQueryRequest request,
		CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		string url = BuildUrl(ApiRoutes.AdminInviteRoutes.Root, request.ToQueryParameters());
		return await this.GetAsync<InviteListResponse>(url, ct);
	}

	public async Task<InviteSummaryResponse> GetInviteSummaryAsync(
		GetInviteSummaryRequest request,
		CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		string url = BuildUrl(ApiRoutes.AdminInviteRoutes.Summary, request.ToQueryParameters());
		return await this.GetAsync<InviteSummaryResponse>(url, ct);
	}

	public async Task<AdminInviteDetailsResponse> GetInviteDetailsAsync(
		Guid inviteId,
		CancellationToken ct = default)
	{
		if (inviteId == Guid.Empty)
		{
			throw new ArgumentException("InviteId is required.", nameof(inviteId));
		}

		string url = BuildUrl(ApiRoutes.AdminInviteRoutes.Details(inviteId));
		return await this.GetAsync<AdminInviteDetailsResponse>(url, ct);
	}

	public async Task<CreateInviteResponse> InviteUserAsync(
		CreateInviteRequest request,
		CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		string url = BuildUrl(ApiRoutes.AdminInviteRoutes.New);
		return await this.PostJsonAsync<CreateInviteRequest, CreateInviteResponse>(url, request, ct);
	}

	public async Task<bool> RevokeInviteAsync(
		Guid inviteId,
		CancellationToken ct = default)
	{
		if (inviteId == Guid.Empty)
		{
			throw new ArgumentException("InviteId is required.", nameof(inviteId));
		}

		string url = BuildUrl(ApiRoutes.AdminInviteRoutes.InviteRevoke(inviteId));
		try
		{
			await this.PostAsync(url, ct);
			return true;
		}
		catch (PortiforceApiException)
		{
			return false;
		}
	}

	public async Task<AccountListResponse> GetUsersAsync(
		GetAccountListQueryRequest request,
		CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		string url = BuildUrl(ApiRoutes.Accounts.Root, request.ToQueryParameters());
		return await this.GetAsync<AccountListResponse>(url, ct);
	}

	public async Task<AccountDetailsResponse> GetUserDetailsAsync(
		Guid accountId,
		CancellationToken ct = default)
	{
		if (accountId == Guid.Empty)
		{
			throw new ArgumentException("AccountId is required.", nameof(accountId));
		}

		string url = BuildUrl(ApiRoutes.Accounts.Details(accountId));
		return await this.GetAsync<AccountDetailsResponse>(url, ct);
	}
}