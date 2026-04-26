using Microsoft.EntityFrameworkCore;

using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.BackgroundTasks.Outbox;

public sealed class OutboxMessageReadRepository(AssetAssistantDbContext db) : IOutboxMessageReadRepository
{
	public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct) =>
		await db.OutboxMessages
			.SingleOrDefaultAsync(x => x.Id == id, ct);

	public async Task<IReadOnlyList<OutboxMessage>> GetReadyToProcessAsync(
		string type,
		DateTimeOffset nowUtc,
		int batchSize,
		CancellationToken ct)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(type);

		if (batchSize <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");
		}

		return await db.OutboxMessages
			.Where(x =>
				x.Type == type &&
				(x.State == OutboxMessageState.Pending || x.State == OutboxMessageState.Failed) &&
				x.NextAttemptAtUtc != null &&
				x.NextAttemptAtUtc <= nowUtc)
			.OrderBy(x => x.NextAttemptAtUtc)
			.ThenBy(x => x.CreatedAtUtc)
			.Take(batchSize)
			.ToListAsync(ct);
	}
}
