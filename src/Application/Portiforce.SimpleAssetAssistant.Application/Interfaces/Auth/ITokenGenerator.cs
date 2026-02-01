using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth.Models;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth;

public interface ITokenGenerator
{
	/// <summary>
	/// Returns the JWT string
	/// </summary>
	/// <param name="accountInfo">account info model</param>
	/// <returns></returns>
	string Generate(IAccountInfo accountInfo);
}