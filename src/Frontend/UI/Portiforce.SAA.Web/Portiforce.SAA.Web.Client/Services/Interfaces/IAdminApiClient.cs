using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.Models.Client.User;
using GetInviteDetailsRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteDetailsRequest;
using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IAdminApiClient
{
	Task<InviteListResponse> GetInvitesAsync(GetInviteListQueryRequest request, CancellationToken ct = default);

	Task<InviteDetailsResponse> GetInviteDetailsAsync(GetInviteDetailsRequest request, CancellationToken ct = default);

	Task<CreateInviteResponse> InviteUserAsync(CreateInviteRequest request, CancellationToken ct = default);

	Task<AccountListResponse> GetUsersAsync(GetAccountListQueryRequest request, CancellationToken ct = default);

	Task<AccountDetailsResponse> GetUserDetailsAsync(GetAccountDetailsRequest request, CancellationToken ct = default);
}
