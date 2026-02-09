using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.Auth;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
	public AccountId Id
	{
		get
		{
			var idClaim = GetClaim(ClaimTypes.NameIdentifier) ?? GetClaim(CustomClaimTypes.UserId);
			return !string.IsNullOrWhiteSpace(idClaim)
				? new AccountId(Guid.Parse(idClaim))
				: AccountId.Empty;
		}
	}

	public TenantId TenantId
	{
		get
		{
			var tenantClaim = GetClaim(CustomClaimTypes.TenantId) ?? GetClaim(CustomClaimTypes.TenantId);
			return !string.IsNullOrWhiteSpace(tenantClaim)
				? new TenantId(Guid.Parse(tenantClaim))
				: TenantId.Empty;
		}
	}

	public Role Role
	{
		get
		{
			var roleClaim = GetClaim(ClaimTypes.Role) ?? GetClaim(CustomClaimTypes.RoleId) ?? GetClaim(CustomClaimTypes.RoleId);

			return !string.IsNullOrWhiteSpace(roleClaim) && Enum.TryParse<Role>(roleClaim, out var role)
				? role
				: Role.None;
		}
	}

	public AccountState State
	{
		get
		{
			var stateClaim = GetClaim(CustomClaimTypes.State);
			return !string.IsNullOrWhiteSpace(stateClaim) && Enum.TryParse<AccountState>(stateClaim, out var state)
				? state
				: AccountState.Unknown;
		}
	}

	public bool IsAuthenticated =>
		httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

	private string? GetClaim(string claimType)
	{
		return httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
	}
}
