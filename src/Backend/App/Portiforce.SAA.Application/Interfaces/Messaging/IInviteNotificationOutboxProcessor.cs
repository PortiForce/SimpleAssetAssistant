namespace Portiforce.SAA.Application.Interfaces.Notification;

public interface IInviteNotificationOutboxProcessor
{
	Task<int> ProcessReadyInviteEmailsAsync(
		int batchSize,
		int maxAttempts,
		CancellationToken ct);
}
