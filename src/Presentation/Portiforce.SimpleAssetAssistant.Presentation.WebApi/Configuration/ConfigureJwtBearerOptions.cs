using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portiforce.SimpleAssetAssistant.Core.Identity;
using Portiforce.SimpleAssetAssistant.Infrastructure.Configuration;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Configuration;

public sealed class ConfigureJwtBearerOptions : IConfigureOptions<JwtBearerOptions>
{
	private readonly IOptions<JwtSettings> _jwt;

	public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwt) => _jwt = jwt;

	public void Configure(JwtBearerOptions options)
	{
		var jwt = _jwt.Value;

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwt.Issuer,
			ValidateAudience = true,
			ValidAudience = jwt.Audience,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
			ValidateLifetime = true,
			ClockSkew = TimeSpan.FromMinutes(1),
			NameClaimType = JwtRegisteredClaimNames.Sub,
			RoleClaimType = CustomClaimTypes.RoleId
		};
	}
}