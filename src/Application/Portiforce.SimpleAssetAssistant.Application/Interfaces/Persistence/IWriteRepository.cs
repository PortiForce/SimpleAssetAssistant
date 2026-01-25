using Portiforce.SimpleAssetAssistant.Core.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;

public interface IWriteRepository<T, TId> where T : IEntity<TId>, IAggregateRoot
{
	Task AddAsync(T entity, CancellationToken ct);
}
