namespace Portiforce.SimpleAssetAssistant.Application.Exceptions
{
	public class BadRequestException : Exception
	{
		public BadRequestException(string code) : base(code)
		{

		}
	}
}
