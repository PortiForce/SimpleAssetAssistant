namespace Portiforce.SimpleAssetAssistant.Application.Result
{
	public record BaseCommandResult
	{
		public bool IsSuccessful { get; init;  } = true;

		public string Message { get; init; } = string.Empty;

		public List<ResultMessageModel> MessageList { get; init; } = new();
	}
}
