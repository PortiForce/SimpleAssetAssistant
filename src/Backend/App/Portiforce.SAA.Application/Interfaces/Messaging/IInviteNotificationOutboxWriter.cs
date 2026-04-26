using Portiforce.SAA.Application.Models.Invite;

namespace Portiforce.SAA.Application.Interfaces.Notification;

public interface IInviteNotificationOutboxWriter
{
	ValueTask AddInviteEmailAsync(
		SendInviteByChannelMessage message,
		CancellationToken ct);
}