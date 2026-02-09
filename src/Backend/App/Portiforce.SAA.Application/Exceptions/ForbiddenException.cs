namespace Portiforce.SAA.Application.Exceptions;

public sealed class ForbiddenException : Exception
{
	public ForbiddenException(string message = "Access denied") : base(message: message)
	{

	}
}
