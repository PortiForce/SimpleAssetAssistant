using Portiforce.SAA.Application.Models.Common.Messaging;

namespace Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;

public interface IInboxMessageWriteRepository
{
	Task AddAsync(InboxMessage entity, CancellationToken ct);
}