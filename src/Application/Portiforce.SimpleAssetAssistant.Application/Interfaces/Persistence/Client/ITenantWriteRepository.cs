using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;

public interface ITenantWriteRepository : IWriteRepository<Tenant, TenantId>
{
	Task UpdateAsync(Tenant tenant, CancellationToken ct);
}
