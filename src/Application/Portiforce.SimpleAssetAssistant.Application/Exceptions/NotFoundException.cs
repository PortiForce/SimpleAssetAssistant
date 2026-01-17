namespace Portiforce.SimpleAssetAssistant.Application.Exceptions;

public class NotFoundException : Exception
{
	public NotFoundException(string entityName, object key) : base($"{entityName}, {key} is not found")
	{

	}
}
