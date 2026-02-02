using Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Tenant;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Tenant;

public interface ITenantLimitsService
{
	Task EnsureTenantCanInviteOrActivateUserAsync(ITenantInfo tenantInfo, CancellationToken ct);
}
