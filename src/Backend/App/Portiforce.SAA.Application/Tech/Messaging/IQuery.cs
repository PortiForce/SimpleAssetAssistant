namespace Portiforce.SAA.Application.Tech.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{

}