namespace Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

// delegate used by IPipelineBehavior
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>();
