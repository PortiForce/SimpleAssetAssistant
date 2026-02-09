namespace Portiforce.SAA.Application.Tech.Messaging;

public interface IMediator
{
	ValueTask<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
}
