namespace Portiforce.SAA.Application.Tech.Abstractions.Messaging;

public interface INotificationHandler<in TNotification>
	where TNotification : INotification
{
	ValueTask Handle(TNotification notification, CancellationToken ct);
}
