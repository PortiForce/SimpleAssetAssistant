namespace Portiforce.SAA.Application.Tech.Messaging;

public interface IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	ValueTask<TResponse> Handle(
		TRequest request,
		CancellationToken ct,
		RequestHandlerDelegate<TResponse> next);
}
