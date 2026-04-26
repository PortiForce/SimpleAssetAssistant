using Portiforce.SAA.Application.Models.Invite;

namespace Portiforce.SAA.Application.Interfaces.Notification;

public interface IInviteNotificationOutboxWriter
{
	ValueTask AddInviteEmailAsync(
		SendInviteEmailMessage message,
		CancellationToken ct);
}