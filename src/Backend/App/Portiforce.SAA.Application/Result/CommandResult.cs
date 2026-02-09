namespace Portiforce.SAA.Application.Result;

public record CommandResult<T> : BaseCommandResult
{
	public T? Id { get; init; } = default;
}
