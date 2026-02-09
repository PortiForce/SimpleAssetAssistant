using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Portiforce.SAA.Application.Tech.Messaging;

/// <summary>
/// todo tech: generate expression trees for the invokers, or make a typed generic mediator.
/// </summary>
/// <param name="sp"></param>
internal sealed class Mediator(IServiceProvider sp) : IMediator, IPublisher
{
	private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object, CancellationToken, ValueTask<object>>> _sendCache = new();
	private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object, CancellationToken, ValueTask>> _publishCache = new();

	public async ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var requestType = request.GetType();
		var invoker = _sendCache.GetOrAdd(requestType, BuildSendInvoker);

		var resultObj = await invoker(sp, request, ct).ConfigureAwait(false);
		return (TResponse)resultObj;
	}

	public ValueTask Publish<TNotification>(TNotification notification, CancellationToken ct = default)
		where TNotification : INotification
	{
		ArgumentNullException.ThrowIfNull(notification);

		var notificationType = notification.GetType();
		var invoker = _publishCache.GetOrAdd(notificationType, BuildPublishInvoker);

		return invoker(sp, notification, ct);
	}

	private static Func<IServiceProvider, object, CancellationToken, ValueTask<object>> BuildSendInvoker(Type requestRuntimeType)
	{
		// We build: sp => (request, ct) => pipeline(handler(request, ct))
		// but typed by runtime request type.

		// Handler: IRequestHandler<TRequest, TResponse>
		// Pipeline: IPipelineBehavior<TRequest, TResponse>[]

		// We must keep it reflection-based ONCE, then cached.
		return async (serviceProvider, requestObj, ct) =>
		{
			// Determine TResponse from IRequest<TResponse> implemented by runtime request type
			// Your IRequest<TResponse> is marker interface; easiest is: get the single IRequest<> interface.
			var ireq = requestRuntimeType.GetInterfaces()
				.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
			var responseType = ireq.GetGenericArguments()[0];

			var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestRuntimeType, responseType);
			var handler = serviceProvider.GetRequiredService(handlerType);

			var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestRuntimeType, responseType);
			var behaviors = serviceProvider.GetServices(behaviorType).Reverse().ToArray();

			// Build "next" delegate: () => handler.Handle((TRequest)request, ct)
			// Use reflection invoke once per call, but without dynamic binder.
			RequestHandlerDelegate<object> invokeHandler = () =>
			{
				var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle))!;
				var taskObj = method.Invoke(handler, [requestObj, ct])!; // ValueTask<TResponse>
				return ConvertValueTaskToObject(taskObj, responseType);
			};

			// Wrap behaviors
			foreach (var b in behaviors)
			{
				var next = invokeHandler;
				invokeHandler = () =>
				{
					var method = behaviorType.GetMethod(nameof(IPipelineBehavior<IRequest<object>, object>.Handle))!;
					var taskObj = method.Invoke(b, [requestObj, ct, next])!; // ValueTask<TResponse>
					return ConvertValueTaskToObject(taskObj, responseType);
				};
			}

			return await invokeHandler().ConfigureAwait(false);
		};
	}

	private static Func<IServiceProvider, object, CancellationToken, ValueTask> BuildPublishInvoker(Type notificationRuntimeType)
	{
		return async (serviceProvider, notificationObj, ct) =>
		{
			var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationRuntimeType);
			var handlers = serviceProvider.GetServices(handlerType);

			var method = handlerType.GetMethod(nameof(INotificationHandler<INotification>.Handle))!;
			foreach (var h in handlers)
			{
				var vt = (ValueTask)method.Invoke(h, [notificationObj, ct])!;
				await vt.ConfigureAwait(false);
			}
		};
	}

	private static async ValueTask<object> ConvertValueTaskToObject(object valueTaskObj, Type responseType)
	{
		// valueTaskObj is ValueTask<TResponse> boxed.
		// We await it via reflection once per call (still cheaper than dynamic binder).
		var asTaskMethod = valueTaskObj.GetType().GetMethod("AsTask")!;
		var task = (Task)asTaskMethod.Invoke(valueTaskObj, Array.Empty<object>())!;
		await task.ConfigureAwait(false);

		// Task<TResponse>.Result
		var resultProp = task.GetType().GetProperty("Result")!;
		return resultProp.GetValue(task)!;
	}
}
