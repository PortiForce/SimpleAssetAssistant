using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Infrastructure.Configuration;

namespace Portiforce.SAA.Infrastructure.Auth;

public class JwtTokenGenerator : ITokenGenerator
{
	public JwtTokenGenerator(IOptions<JwtSettings> settings)
	{
		_settings = settings.Value;
	}
	private readonly JwtSettings _settings;

	public string GenerateAccessToken(IAccountInfo accountInfo)
	{
		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, accountInfo.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, accountInfo.Email),
			new(CustomClaimTypes.TenantId, accountInfo.TenantId.ToString()),
			new(CustomClaimTypes.RoleId, accountInfo.Role.ToString()),
			new(CustomClaimTypes.State, accountInfo.State.ToString())
		];

		SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
		SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		JwtSecurityToken token = new JwtSecurityToken(
			issuer: _settings.Issuer,
			audience: _settings.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		Span<byte> bytes = stackalloc byte[32];
		RandomNumberGenerator.Fill(bytes);
		return WebEncoders.Base64UrlEncode(bytes);
	}
}
