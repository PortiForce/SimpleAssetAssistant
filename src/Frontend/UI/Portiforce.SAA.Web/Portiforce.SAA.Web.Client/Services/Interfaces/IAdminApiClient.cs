using Portiforce.SAA.Contracts.Models.Client.Account;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using GetInviteListQueryRequest = Portiforce.SAA.Contracts.Models.Client.Invite.GetInviteListQueryRequest;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IAdminApiClient
{
	Task<InviteListResponse> GetInvitesAsync(GetInviteListQueryRequest request, CancellationToken ct = default);

	Task<InviteDetailsResponse> GetInviteDetailsAsync(Guid inviteId, CancellationToken ct = default);

	Task<CreateInviteResponse> InviteUserAsync(CreateInviteRequest request, CancellationToken ct = default);

	Task<bool> RevokeInviteAsync(Guid inviteId, CancellationToken ct = default);

	Task<AccountListResponse> GetUsersAsync(GetAccountListQueryRequest request, CancellationToken ct = default);

	Task<AccountDetailsResponse> GetUserDetailsAsync(Guid accountId, CancellationToken ct = default);
}
