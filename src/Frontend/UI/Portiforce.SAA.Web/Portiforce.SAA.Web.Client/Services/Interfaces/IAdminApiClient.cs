using Portiforce.SAA.Contracts.Models.Invite;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IAdminApiClient
{
	Task<CreateInviteResponse> InviteUserAsync(CreateInviteRequest request, CancellationToken ct = default);
}
