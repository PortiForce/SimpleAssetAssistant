using Microsoft.Extensions.Logging;

using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Notification;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Serialization;

namespace Portiforce.SAA.Notifications.Worker.Invite;

public sealed class InviteNotificationOutboxProcessor(
	IOutboxMessageReadRepository outboxMessageReadRepository,
	IInviteChannelSender inviteChannelSender,
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

		int processedCount = 0;

		foreach (OutboxMessage outboxMessage in messages)
		{
			ct.ThrowIfCancellationRequested();

			try
			{
				SendInviteByChannelMessage message =
					jsonSerializer.Deserialize<SendInviteByChannelMessage>(outboxMessage.PayloadJson);

				InviteSendResult result = await inviteChannelSender.SendAsync(message, ct);
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