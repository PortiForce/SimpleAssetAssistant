using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.BackgroundTasks.Inbox;

internal sealed class InboxMessageWriteRepository(AssetAssistantDbContext db) : IInboxMessageWriteRepository
{
	public async Task AddAsync(InboxMessage entity, CancellationToken ct) =>
		await db.InboxMessages.AddAsync(entity, ct);
}