namespace Portiforce.SimpleAssetAssistant.Core.Identity.Exceptions;

public sealed class InvalidRefreshTokenException : Exception
{
	public InvalidRefreshTokenException(string message) : base(message)
	{

	}
}
