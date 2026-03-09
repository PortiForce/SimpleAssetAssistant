namespace Portiforce.SAA.Application.Tech.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{

}