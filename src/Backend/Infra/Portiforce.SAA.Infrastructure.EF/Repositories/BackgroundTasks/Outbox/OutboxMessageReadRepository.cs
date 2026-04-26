using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Models.Common.Messaging;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.BackgroundTasks.Outbox;

public sealed class OutboxMessageReadRepository : IOutboxMessageReadRepository
{
	public Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct) => throw new NotImplementedException();
}