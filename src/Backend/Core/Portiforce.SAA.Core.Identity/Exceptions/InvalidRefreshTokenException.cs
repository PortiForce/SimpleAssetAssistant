namespace Portiforce.SAA.Core.Identity.Exceptions;

public sealed class InvalidRefreshTokenException : Exception
{
	public InvalidRefreshTokenException(string message) : base(message)
	{

	}
}
