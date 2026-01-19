using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;

public interface IReadRepository<TDetails, in TId>
	where TDetails : IDetailsProjection
{
	Task<TDetails?> GetByIdAsync(TId id, CancellationToken ct);
}