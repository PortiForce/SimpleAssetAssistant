using Microsoft.Extensions.DependencyInjection;

namespace Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

internal sealed class Mediator(IServiceProvider serviceProvider) : IMediator, IPublisher
{
	public async ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var requestType = request.GetType();
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
		var handler = serviceProvider.GetRequiredService(handlerType);

		// Build pipeline: IPipelineBehavior<TRequest,TResponse>[]
		var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
		var behaviors = serviceProvider.GetServices(behaviorType).Reverse().ToArray();

		RequestHandlerDelegate<TResponse> invokeHandler = () =>
		{
			// dynamic call into strongly typed handler
			return ((dynamic)handler).Handle((dynamic)request, ct);
		};

		foreach (var b in behaviors)
		{
			var next = invokeHandler;
			invokeHandler = () => ((dynamic)b).Handle((dynamic)request, ct, next);
		}

		return await invokeHandler().ConfigureAwait(false);
	}

	public async ValueTask Publish<TNotification>(TNotification notification, CancellationToken ct = default)
		where TNotification : INotification
	{
		ArgumentNullException.ThrowIfNull(notification);

		// multiple handlers
		var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
		var handlers = serviceProvider.GetServices(handlerType);

		foreach (var h in handlers)
		{
			await ((dynamic)h).Handle((dynamic)notification, ct).ConfigureAwait(false);
		}
	}
}
