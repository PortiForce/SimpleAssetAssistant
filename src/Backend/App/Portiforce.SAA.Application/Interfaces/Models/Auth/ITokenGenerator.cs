namespace Portiforce.SAA.Application.Interfaces.Models.Auth;

public interface ITokenGenerator
{
	/// <summary>
	/// Returns the JWT string
	/// </summary>
	/// <param name="accountInfo">account info model</param>
	/// <returns></returns>
	string GenerateAccessToken(IAccountInfo accountInfo);

	string GenerateRefreshToken();
}