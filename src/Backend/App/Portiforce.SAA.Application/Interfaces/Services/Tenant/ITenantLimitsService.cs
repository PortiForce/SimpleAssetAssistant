using Portiforce.SAA.Application.Interfaces.Models.Tenant;

namespace Portiforce.SAA.Application.Interfaces.Services.Tenant;

public interface ITenantLimitsService
{
	Task EnsureTenantCanInviteOrActivateUserAsync(ITenantInfo tenantInfo, CancellationToken ct);
}
