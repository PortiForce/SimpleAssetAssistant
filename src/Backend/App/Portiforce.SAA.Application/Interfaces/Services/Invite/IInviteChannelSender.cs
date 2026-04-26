using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.Interfaces.Services.Invite;

public interface IInviteChannelSender
{
	InviteChannel Channel { get; }

	Task<InviteSendResult> SendAsync(
		SendInviteByChannelMessage message,
		CancellationToken ct);
}