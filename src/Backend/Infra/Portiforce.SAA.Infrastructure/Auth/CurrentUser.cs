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
			string? idClaim = this.GetClaim(ClaimTypes.NameIdentifier) ?? this.GetClaim(CustomClaimTypes.UserId);
			return !string.IsNullOrWhiteSpace(idClaim)
				? new AccountId(Guid.Parse(idClaim))
				: AccountId.Empty;
		}
	}

	public TenantId TenantId
	{
		get
		{
			string? tenantClaim = this.GetClaim(CustomClaimTypes.TenantId) ?? this.GetClaim(CustomClaimTypes.TenantId);
			return !string.IsNullOrWhiteSpace(tenantClaim)
				? new TenantId(Guid.Parse(tenantClaim))
				: TenantId.Empty;
		}
	}

	public Role Role
	{
		get
		{
			string? roleClaim = this.GetClaim(ClaimTypes.Role) ??
								this.GetClaim(CustomClaimTypes.RoleId) ?? this.GetClaim(CustomClaimTypes.RoleId);

			return !string.IsNullOrWhiteSpace(roleClaim) && Enum.TryParse(roleClaim, out Role role)
				? role
				: Role.None;
		}
	}

	public AccountState State
	{
		get
		{
			string? stateClaim = this.GetClaim(CustomClaimTypes.State);
			return !string.IsNullOrWhiteSpace(stateClaim) &&
				   Enum.TryParse(stateClaim, out AccountState state)
				? state
				: AccountState.Unknown;
		}
	}

	public bool IsAuthenticated =>
		httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

	private string? GetClaim(string claimType) => httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
}