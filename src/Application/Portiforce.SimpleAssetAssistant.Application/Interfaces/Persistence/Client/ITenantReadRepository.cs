using Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;

public interface ITenantReadRepository : IReadRepository<TenantDetails, TenantId>
{
	
}
