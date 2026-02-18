using Portiforce.SAA.Application.Exceptions;

namespace Portiforce.SAA.Application.FlowResult;

public readonly record struct TypedResult<T>
{
	public bool IsSuccess { get; }
	public T? Value { get; }
	public ResultError? Error { get; }

	private TypedResult(bool isSuccess, T? value, ResultError? error)
	{
		IsSuccess = isSuccess;
		Value = value;
		Error = error;
	}

	public static TypedResult<T> Ok(T value) => new(true, value, null);
	public static TypedResult<T> Fail(ResultError error) => new(false, default, error);
}
