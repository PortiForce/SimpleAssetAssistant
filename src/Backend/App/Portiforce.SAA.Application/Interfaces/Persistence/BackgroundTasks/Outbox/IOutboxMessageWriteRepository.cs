using Portiforce.SAA.Application.Models.Common.Messaging;

namespace Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;

public interface IOutboxMessageWriteRepository
{
	Task AddAsync(OutboxMessage entity, CancellationToken ct);
}