using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services.ApiClients;

public sealed class PublicApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IPublicApiClient
{
	public async Task<DeclineInviteResponse> DeclineInviteAsync(
		string inviteToken,
		CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(inviteToken);

		string url = ApiRoutes.PublicRoutes.DeclineInvite(Uri.EscapeDataString(inviteToken));

		return await this.PostAsync<DeclineInviteResponse>(url, ct);
	}
}