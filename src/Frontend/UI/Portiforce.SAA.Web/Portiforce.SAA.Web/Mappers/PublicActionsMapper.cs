using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Models.Client.Invite;

namespace Portiforce.SAA.Web.Mappers;

public static class PublicActionsMapper
{
	public static DeclineInviteResponse MapToResponse(this DeclineInviteResult declineInviteResult) => new();
}