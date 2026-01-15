using Portiforce.SimpleAssetAssistant.Core.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;

public interface IRepository<T, TId> where T : IEntity<TId>, IAggregateRoot
{
	Task<T?> GetByIdAsync(TId id, CancellationToken ct);

	Task AddAsync(T entity, CancellationToken ct);
}