namespace Portiforce.SAA.Notifications.Worker.Host;

public sealed class InviteNotificationWorkerOptions
{
	public const string SectionName = "InviteNotificationWorker";

	public int BatchSize { get; set; } = 25;

	public int MaxAttempts { get; set; } = 5;

	public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(30);

	public bool RunImmediately { get; set; } = true;

	public static bool IsValid(InviteNotificationWorkerOptions options) =>
		options.BatchSize is > 0 and <= 1_000 &&
		options.MaxAttempts is > 0 and <= 100 &&
		options.PollInterval >= TimeSpan.FromSeconds(1);
}
