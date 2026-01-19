using Portiforce.SimpleAssetAssistant.Application.Enums;

namespace Portiforce.SimpleAssetAssistant.Application.Result
{
	public sealed record ResultMessageModel
	{
		public ResponseType ResponseType { get; init; }

		public string? Code { get; init; }

		public string? Message { get; init; }
	}
}
