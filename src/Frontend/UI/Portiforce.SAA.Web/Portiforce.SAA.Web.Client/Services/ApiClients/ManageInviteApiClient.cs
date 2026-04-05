using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services.ApiClients;

public sealed class ManageInviteApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IManageInviteApiClient
{
	public async Task<OverviewInviteDetailsResponse> GetInviteOverviewAsync(
		string inviteToken,
		CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(inviteToken);

		string url = ApiRoutes.PublicInviteRoutes.OverviewInvite(Uri.EscapeDataString(inviteToken));

		return await this.GetAsync<OverviewInviteDetailsResponse>(url, ct);
	}

	public async Task<DeclineInviteResponse> DeclineInviteAsync(
		string inviteToken,
		CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(inviteToken);

		string url = ApiRoutes.PublicInviteRoutes.DeclineInvite(Uri.EscapeDataString(inviteToken));

		return await this.PostAsync<DeclineInviteResponse>(url, ct);
	}

	public async Task<string> InitAcceptInviteAsync(string inviteToken, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(inviteToken);

		string url = ApiRoutes.PublicInviteRoutes.InitAcceptInvite(Uri.EscapeDataString(inviteToken));

		return await this.PostAsync<string>(url, ct);
	}
}