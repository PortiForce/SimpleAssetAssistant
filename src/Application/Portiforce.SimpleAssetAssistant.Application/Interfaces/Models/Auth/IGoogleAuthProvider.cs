using Portiforce.SimpleAssetAssistant.Application.Models.Auth;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Auth;

public interface IGoogleAuthProvider
{
	/// <summary>
	/// Verifies the token with Google and returns user info
	/// </summary>
	/// <param name="idToken"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	Task<GoogleUserInfo> VerifyAsync(string idToken, CancellationToken ct);
}
