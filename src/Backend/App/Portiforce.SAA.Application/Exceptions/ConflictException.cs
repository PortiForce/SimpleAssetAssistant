namespace Portiforce.SAA.Application.Exceptions;

public sealed class ConflictException : Exception
{
	public ConflictException(string message) : base(message: message)
	{

	}
}
