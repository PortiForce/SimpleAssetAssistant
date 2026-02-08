namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Security;

public interface IHashingService
{
	public byte[] HashRefreshToken(string rawValue);
}
