using Portiforce.SAA.Contracts.Models.Client.Invite;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IManageInviteApiClient
{
	Task<OverviewInviteDetailsResponse> GetInviteOverviewAsync(
		string inviteToken,
		CancellationToken ct = default);

	Task<DeclineInviteResponse> DeclineInviteAsync(
		string inviteToken,
		CancellationToken ct = default);

	Task<string> InitAcceptInviteAsync(
		string inviteToken,
		CancellationToken ct = default);
}