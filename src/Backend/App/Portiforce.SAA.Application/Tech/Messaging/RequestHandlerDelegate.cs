namespace Portiforce.SAA.Application.Tech.Messaging;

// delegate used by IPipelineBehavior
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>();
