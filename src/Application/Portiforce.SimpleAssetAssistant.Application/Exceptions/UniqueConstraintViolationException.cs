namespace Portiforce.SimpleAssetAssistant.Application.Exceptions;

public sealed class UniqueConstraintViolationException : Exception
{
	public UniqueConstraintViolationException(string message = "constraint violation") : base(message: message)
	{

	}
}
