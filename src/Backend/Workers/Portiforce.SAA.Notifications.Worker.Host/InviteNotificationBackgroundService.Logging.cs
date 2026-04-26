namespace Portiforce.SAA.Notifications.Worker.Host;

public sealed partial class InviteNotificationBackgroundService
{
	[LoggerMessage(
		EventId = 1000,
		Level = LogLevel.Information,
		Message = "Invite notification worker started. PollInterval: {PollInterval}, BatchSize: {BatchSize}, MaxAttempts: {MaxAttempts}.")]
	private static partial void LogWorkerStarted(
		ILogger logger,
		TimeSpan pollInterval,
		int batchSize,
		int maxAttempts);

	[LoggerMessage(
		EventId = 1001,
		Level = LogLevel.Information,
		Message = "Invite notification worker is stopping.")]
	private static partial void LogWorkerStopping(ILogger logger);

	[LoggerMessage(
		EventId = 1002,
		Level = LogLevel.Information,
		Message = "Processed {ProcessedCount} invite email outbox message(s).")]
	private static partial void LogProcessedInviteEmailOutboxMessages(ILogger logger, int processedCount);

	[LoggerMessage(
		EventId = 1003,
		Level = LogLevel.Debug,
		Message = "No invite email outbox messages were ready to process.")]
	private static partial void LogNoInviteEmailOutboxMessagesReady(ILogger logger);

	[LoggerMessage(
		EventId = 1004,
		Level = LogLevel.Error,
		Message = "Invite email outbox processing failed. The worker will retry on the next polling interval.")]
	private static partial void LogInviteEmailOutboxProcessingFailed(ILogger logger, Exception exception);
}
