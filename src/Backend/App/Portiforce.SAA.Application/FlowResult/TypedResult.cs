using Portiforce.SAA.Application.Exceptions;

namespace Portiforce.SAA.Application.FlowResult;

public readonly record struct TypedResult<T>
{
	private TypedResult(bool isSuccess, T? value, ResultError? error)
	{
		this.IsSuccess = isSuccess;
		this.Value = value;
		this.Error = error;
	}

	public bool IsSuccess { get; }

	public T? Value { get; }

	public ResultError? Error { get; }

	public static TypedResult<T> Ok(T value) => new(true, value, null);

	public static TypedResult<T> Fail(ResultError error) => new(false, default, error);
}