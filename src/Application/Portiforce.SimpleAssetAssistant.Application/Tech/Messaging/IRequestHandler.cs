namespace Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

public interface IRequestHandler<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	ValueTask<TResponse> Handle(TRequest request, CancellationToken ct);
}
