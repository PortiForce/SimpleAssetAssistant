namespace Portiforce.SAA.Application.Tech.Messaging;

public interface INotificationHandler<in TNotification>
	where TNotification : INotification
{
	ValueTask Handle(TNotification notification, CancellationToken ct);
}
