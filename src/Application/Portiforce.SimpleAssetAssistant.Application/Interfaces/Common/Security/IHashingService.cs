namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Security;

public interface IHashingService
{
	public string HashRefreshToken(string rawValue);
}
