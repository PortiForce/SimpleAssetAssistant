using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Models.Tenant;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Services.Tenant;

public interface ITenantLimitsService
{
	Task<Result> EnsureTenantCanInviteOrActivateAccountAsync(TenantId tenantId, CancellationToken ct);

	Task<Result> EnsureTenantInvitesAndAccountLimitsAsync(ITenantInfo tenantInfo, CancellationToken ct);
}
