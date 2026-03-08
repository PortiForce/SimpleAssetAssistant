namespace Portiforce.SAA.Application.Tech.Abstractions.Messaging;

public interface IMediator
{
	ValueTask<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
}
