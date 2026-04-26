namespace Portiforce.SAA.Application.Interfaces.Messaging;

public interface IInviteNotificationOutboxProcessor
{
    Task<int> ProcessReadyInviteEmailsAsync(
        int batchSize,
        int maxAttempts,
        CancellationToken ct);
}