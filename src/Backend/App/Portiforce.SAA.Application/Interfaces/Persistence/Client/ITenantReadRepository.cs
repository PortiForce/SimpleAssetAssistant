using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Client;

public interface ITenantReadRepository : IReadRepository<TenantDetails, TenantId>
{
	/// <summary>
	/// method to use in hot path flows, then tenant important details are needed for quick access
	/// </summary>
	/// <param name="id"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	Task<TenantSummary?> GetSummaryByIdAsync(TenantId id, CancellationToken ct);
}
