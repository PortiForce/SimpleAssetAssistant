namespace Portiforce.SimpleAssetAssistant.Application.Result;

public record CommandResult<T> : BaseCommandResult
{
	public T? Id { get; init; } = default;
}
