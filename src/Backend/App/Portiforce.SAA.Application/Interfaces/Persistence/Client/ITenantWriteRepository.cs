using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Client;

public interface ITenantWriteRepository : IWriteRepository<Tenant, TenantId>
{
	Task UpdateAsync(Tenant tenant, CancellationToken ct);
}
