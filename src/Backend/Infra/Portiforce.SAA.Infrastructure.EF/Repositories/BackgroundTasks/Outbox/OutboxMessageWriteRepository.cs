using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.BackgroundTasks.Outbox;

internal sealed class OutboxMessageWriteRepository(AssetAssistantDbContext db) : IOutboxMessageWriteRepository
{
	public async Task AddAsync(OutboxMessage entity, CancellationToken ct) =>
		await db.OutboxMessages.AddAsync(entity, ct);
}