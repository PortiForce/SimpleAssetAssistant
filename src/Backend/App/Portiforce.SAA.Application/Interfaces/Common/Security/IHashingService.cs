namespace Portiforce.SAA.Application.Interfaces.Common.Security;

public interface IHashingService
{
	public byte[] HashRefreshToken(string rawValue);
}
