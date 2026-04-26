using Microsoft.Extensions.Logging;

using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Messaging;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Serialization;

namespace Portiforce.SAA.Notifications.Worker.Invite;

public sealed class InviteNotificationOutboxProcessor(
	IOutboxMessageReadRepository outboxMessageReadRepository,
	IEnumerable<IInviteChannelSender> inviteChannelSenders,
	IJsonSerializer jsonSerializer,
	IClock clock,
	IUnitOfWork unitOfWork,
	ILogger<InviteNotificationOutboxProcessor> logger) : IInviteNotificationOutboxProcessor
{
	private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(5);

	public async Task<int> ProcessReadyInviteEmailsAsync(
		int batchSize,
		int maxAttempts,
		CancellationToken ct)
	{
		if (batchSize <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");
		}

		if (maxAttempts <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be positive.");
		}

		DateTimeOffset now = clock.UtcNow;
		IReadOnlyList<OutboxMessage> messages = await outboxMessageReadRepository.GetReadyToProcessAsync(
			MessageTypes.SendInviteEmailV1,
			now,
			batchSize,
			ct);

		if (messages.Count == 0)
		{
			return 0;
		}

		// Claim the batch by marking all messages as Published before sending.
		// Published acts as an in-progress lock: GetReadyToProcessAsync only queries Pending/Failed,
		// so a second worker instance won't pick up the same messages.
		// Failed sends call MarkFailed, which transitions the message back to Failed (retryable).
		foreach (OutboxMessage msg in messages)
		{
			msg.MarkPublished(now);
		}

		_ = await unitOfWork.SaveChangesAsync(ct);

		int processedCount = 0;

		foreach (OutboxMessage outboxMessage in messages)
		{
			ct.ThrowIfCancellationRequested();

			try
			{
				SendInviteByChannelMessage message =
					jsonSerializer.Deserialize<SendInviteByChannelMessage>(outboxMessage.PayloadJson);

				IInviteChannelSender? sender = inviteChannelSenders
					.FirstOrDefault(s => s.Channel == message.Channel);

				if (sender is null)
				{
					DateTimeOffset noSenderFailedAt = clock.UtcNow;

					logger.LogWarning(
						"No sender registered for channel '{Channel}' on outbox message {OutboxMessageId}.",
						message.Channel,
						outboxMessage.Id);

					outboxMessage.MarkFailed(
						$"No sender registered for channel '{message.Channel}'.",
						noSenderFailedAt,
						noSenderFailedAt.Add(RetryDelay),
						maxAttempts);

					continue;
				}

				InviteSendResult result = await sender.SendAsync(message, ct);
				DateTimeOffset completedAtUtc = clock.UtcNow;

				if (result.IsSuccess)
				{
					outboxMessage.MarkProcessed(completedAtUtc);
					processedCount++;
					continue;
				}

				outboxMessage.MarkFailed(
					result.ErrorMessage ?? result.ErrorCode ?? "Invite email delivery failed.",
					completedAtUtc,
					completedAtUtc.Add(RetryDelay),
					maxAttempts);
			}
			catch (Exception ex) when (ex is not OperationCanceledException)
			{
				DateTimeOffset failedAtUtc = clock.UtcNow;

				logger.LogWarning(
					ex,
					"Invite email outbox message {OutboxMessageId} failed.",
					outboxMessage.Id);

				outboxMessage.MarkFailed(
					"Invite email outbox message could not be processed.",
					failedAtUtc,
					failedAtUtc.Add(RetryDelay),
					maxAttempts);
			}
		}

		_ = await unitOfWork.SaveChangesAsync(ct);

		return processedCount;
	}
}
