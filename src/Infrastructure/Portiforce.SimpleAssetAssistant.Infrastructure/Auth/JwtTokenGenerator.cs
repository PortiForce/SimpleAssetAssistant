using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth.Models;
using Portiforce.SimpleAssetAssistant.Infrastructure.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.Auth;

public class JwtTokenGenerator : ITokenGenerator
{
	public JwtTokenGenerator(IOptions<JwtSettings> settings)
	{
		_settings = settings.Value;
	}
	private readonly JwtSettings _settings;

	public string Generate(IAccountInfo accountInfo)
	{
		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, accountInfo.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, accountInfo.Email),
			new(CustomClaimTypes.CompanyId, accountInfo.TenantId.ToString()),
			new(CustomClaimTypes.RoleId, accountInfo.Role.ToString())
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
}
