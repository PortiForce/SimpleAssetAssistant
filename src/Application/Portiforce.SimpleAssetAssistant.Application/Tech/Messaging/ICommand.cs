namespace Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{

}
