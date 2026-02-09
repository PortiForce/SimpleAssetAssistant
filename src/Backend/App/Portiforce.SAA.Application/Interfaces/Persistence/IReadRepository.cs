using Portiforce.SAA.Application.Interfaces.Projections;

namespace Portiforce.SAA.Application.Interfaces.Persistence;

public interface IReadRepository<TDetails, in TId>
	where TDetails : IDetailsProjection
{
	Task<TDetails?> GetByIdAsync(TId id, CancellationToken ct);
}