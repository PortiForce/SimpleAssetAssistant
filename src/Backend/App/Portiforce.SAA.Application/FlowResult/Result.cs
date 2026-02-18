using Portiforce.SAA.Application.Exceptions;

namespace Portiforce.SAA.Application.FlowResult;

public readonly record struct Result
{
	public bool IsSuccess { get; }
	public ResultError? Error { get; }

	private Result(bool isSuccess, ResultError? error)
	{
		IsSuccess = isSuccess;
		Error = error;
	}

	public static Result Ok() => new(true, null);
	public static Result Fail(ResultError error) => new(false, error);

	public void ThrowIfFailed()
	{
		if (!IsSuccess)
		{
			throw new InvalidOperationException(Error?.Message ?? "Operation failed.");
		}
	}
}