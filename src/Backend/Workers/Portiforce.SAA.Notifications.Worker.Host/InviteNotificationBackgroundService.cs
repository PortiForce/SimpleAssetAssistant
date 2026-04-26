using Microsoft.Extensions.Options;

using Portiforce.SAA.Application.Interfaces.Notification;

namespace Portiforce.SAA.Notifications.Worker.Host;

public sealed partial class InviteNotificationBackgroundService(
	IServiceScopeFactory scopeFactory,
	IOptions<InviteNotificationWorkerOptions> options,
	ILogger<InviteNotificationBackgroundService> logger)
	: BackgroundService
{
	private readonly InviteNotificationWorkerOptions options = options.Value;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		LogWorkerStarted(
			logger,
			this.options.PollInterval,
			this.options.BatchSize,
			this.options.MaxAttempts);

		try
		{
			if (this.options.RunImmediately)
			{
				await this.ProcessReadyInviteEmailsAsync(stoppingToken);
			}

			using PeriodicTimer timer = new(this.options.PollInterval);

			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				await this.ProcessReadyInviteEmailsAsync(stoppingToken);
			}
		}
		catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
		{
			LogWorkerStopping(logger);
		}
	}

	private async Task ProcessReadyInviteEmailsAsync(CancellationToken stoppingToken)
	{
		try
		{
			using IServiceScope scope = scopeFactory.CreateScope();
			IInviteNotificationOutboxProcessor processor =
				scope.ServiceProvider.GetRequiredService<IInviteNotificationOutboxProcessor>();

			int processedCount = await processor.ProcessReadyInviteEmailsAsync(
				this.options.BatchSize,
				this.options.MaxAttempts,
				stoppingToken);

			if (processedCount > 0)
			{
				LogProcessedInviteEmailOutboxMessages(logger, processedCount);
			}
			else
			{
				LogNoInviteEmailOutboxMessagesReady(logger);
			}
		}
		catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception ex)
		{
			LogInviteEmailOutboxProcessingFailed(logger, ex);
		}
	}
}
