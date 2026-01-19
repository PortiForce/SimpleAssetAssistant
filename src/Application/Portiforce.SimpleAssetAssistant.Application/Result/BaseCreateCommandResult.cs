namespace Portiforce.SimpleAssetAssistant.Application.Result
{
	public record BaseCreateCommandResult<T> : BaseCommandResult
	{
		public T Id { get; init; } = default;
	}
}
