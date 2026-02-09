using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Infrastructure.Configuration;

namespace Portiforce.SAA.Infrastructure.Services.Security;

public sealed class TokenHashingService : IHashingService
{
	private readonly byte[] _key;

	public TokenHashingService(IOptions<TokenHashingOptions> jwt)
		=> _key = Encoding.UTF8.GetBytes(jwt.Value.Pepper);

	public byte[] HashRefreshToken(string rawToken)
	{
		if (string.IsNullOrWhiteSpace(rawToken))
		{
			throw new ArgumentException("Token is required.", nameof(rawToken));
		}

		using var hmac = new HMACSHA256(_key);
		byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawToken));

		return hash;
	}
}
