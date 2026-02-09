using Portiforce.SAA.Core.Interfaces;

namespace Portiforce.SAA.Application.Interfaces.Persistence;

public interface IWriteRepository<T, TId> where T : IEntity<TId>, IAggregateRoot
{
	Task AddAsync(T entity, CancellationToken ct);
}
