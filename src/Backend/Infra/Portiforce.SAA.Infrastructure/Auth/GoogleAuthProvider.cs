using System.Security.Authentication;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Infrastructure.Configuration;

namespace Portiforce.SAA.Infrastructure.Auth;

public class GoogleAuthProvider : IGoogleAuthProvider
{
	private readonly string _clientId;
	
	public GoogleAuthProvider(IOptions<GoogleClientSettings> settings)
	{
		_clientId = settings.Value.ClientId;
	}
	
	public async Task<GoogleUserInfo> VerifyAsync(string idToken, CancellationToken ct)
	{
		try
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings
			{
				Audience = [_clientId]
			};

			var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

			return new GoogleUserInfo(payload.Email, payload.Subject, payload.Name, payload.Picture);
		}
		catch (InvalidJwtException ex)
		{
			throw new AuthenticationException("Invalid Google Token", ex);
		}
	}
}
