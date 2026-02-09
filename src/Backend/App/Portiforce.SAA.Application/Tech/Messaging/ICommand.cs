namespace Portiforce.SAA.Application.Tech.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{

}
