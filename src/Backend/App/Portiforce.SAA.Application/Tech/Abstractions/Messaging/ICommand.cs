namespace Portiforce.SAA.Application.Tech.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{

}
