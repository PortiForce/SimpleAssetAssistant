using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;

namespace Portiforce.SAA.Application.Interfaces.Services.Invite;

public interface IInviteChannelSender
{
	InviteChannel Channel { get; }

	Task<InviteSendResult> SendAsync(
		TenantInvite invite,
		string inviteLink,
		CancellationToken ct);
}