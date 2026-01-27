using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth;

public interface ITokenGenerator
{
	/// <summary>
	/// Returns the JWT string
	/// </summary>
	/// <param name="account"></param>
	/// <returns></returns>
	string Generate(Account account);
}