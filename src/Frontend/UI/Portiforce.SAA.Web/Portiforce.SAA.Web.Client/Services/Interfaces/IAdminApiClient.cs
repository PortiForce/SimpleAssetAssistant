using Portiforce.SAA.Contracts.User.Requests;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IAdminApiClient
{
	Task InviteUserAsync(InviteUserRequest request);
}
