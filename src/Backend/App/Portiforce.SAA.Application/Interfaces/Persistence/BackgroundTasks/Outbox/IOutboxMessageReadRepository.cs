using Portiforce.SAA.Application.Models.Common.Messaging;

namespace Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;

public interface IOutboxMessageReadRepository : IReadRepository<OutboxMessage, Guid>
{
}