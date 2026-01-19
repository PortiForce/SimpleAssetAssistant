namespace Portiforce.SimpleAssetAssistant.Application.Exceptions;

public sealed class BadRequestException : Exception
{
	public BadRequestException(string code) : base(code)
	{

	}
}
