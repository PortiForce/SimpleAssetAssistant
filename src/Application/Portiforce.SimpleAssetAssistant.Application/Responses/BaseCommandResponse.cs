namespace Portiforce.SimpleAssetAssistant.Application.Responses
{
	public class BaseCommandResponse
	{
		public bool IsSuccessful { get; set; } = true;

		public string Message { get; set; }

		public List<ResponseMessageModel> Data { get; set; }
	}
}
