namespace Portiforce.SimpleAssetAssistant.Application.Responses
{
	public class BaseCreateCommandResponse<T> : BaseCommandResponse
	{
		public T Id { get; set; }
	}
}
