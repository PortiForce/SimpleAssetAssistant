namespace Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

public interface IPublisher
{
	ValueTask Publish<TNotification>(TNotification notification, CancellationToken ct = default)
		where TNotification : INotification;
}
