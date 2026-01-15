using Portiforce.SimpleAssetAssistant.Application.Enums;

namespace Portiforce.SimpleAssetAssistant.Application.Responses
{
	public class ResponseMessageModel
	{
		public ResponseType ResponseType { get; set; }

		public string Code { get; set; }

		public string Message { get; set; }
	}
}
