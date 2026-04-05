using Portiforce.SAA.Application.Exceptions;

namespace Portiforce.SAA.Application.FlowResult;

public readonly record struct Result
{
	private Result(bool isSuccess, ResultError? error)
	{
		this.IsSuccess = isSuccess;
		this.Error = error;
	}

	public bool IsSuccess { get; }

	public ResultError? Error { get; }

	public static Result Ok() => new(true, null);

	public static Result Fail(ResultError error) => new(false, error);

	public void ThrowIfFailed()
	{
		if (!this.IsSuccess)
		{
			throw new InvalidOperationException(this.Error?.Message ?? "Operation failed.");
		}
	}
}