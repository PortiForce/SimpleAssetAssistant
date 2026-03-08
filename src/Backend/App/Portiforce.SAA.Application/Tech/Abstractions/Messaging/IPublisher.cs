namespace Portiforce.SAA.Application.Tech.Abstractions.Messaging;

public interface IPublisher
{
	ValueTask Publish<TNotification>(TNotification notification, CancellationToken ct = default)
		where TNotification : INotification;
}
