namespace Portiforce.SAA.Application.Exceptions;

public sealed class NotFoundException : Exception
{
	public NotFoundException(string entityName, object key) : base($"{entityName} {key} was not found")
	{

	}

	public NotFoundException(string details) : base(details)
	{

	}
}
