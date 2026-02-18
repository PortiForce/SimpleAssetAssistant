using Portiforce.SAA.Contracts.Models.Invite;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IAdminApiClient
{
	Task InviteUserAsync(CreateInviteRequest request);
}
