using Portiforce.SAA.Contracts.Models.Client.Invite;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IPublicApiClient
{
	Task<DeclineInviteResponse> DeclineInviteAsync(
		string inviteToken,
		CancellationToken ct = default);
}