using Microsoft.EntityFrameworkCore;

using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.BackgroundTasks.Outbox;

internal sealed class OutboxMessageReadRepository(AssetAssistantDbContext db) : IOutboxMessageReadRepository
{
	public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct) =>
		await db.OutboxMessages
			.SingleOrDefaultAsync(x => x.Id == id, ct);

	public async Task<IReadOnlyList<OutboxMessage>> GetReadyToProcessAsync(
		string type,
		DateTimeOffset nowUtc,
		int batchSize,
		TimeSpan publishedLeaseTimeout,
		CancellationToken ct)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(type);

		if (batchSize <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");
		}

		if (publishedLeaseTimeout <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(publishedLeaseTimeout), "Published lease timeout must be positive.");
		}

		DateTimeOffset leaseExpiredBeforeUtc = nowUtc - publishedLeaseTimeout;

		return await db.OutboxMessages
			.Where(x =>
				x.Type == type &&
				(
					// Normal retry path: Pending or Failed messages due for processing
					((x.State == OutboxMessageState.Pending || x.State == OutboxMessageState.Failed) &&
					 x.NextAttemptAtUtc != null &&
					 x.NextAttemptAtUtc <= nowUtc) ||
					// Lease recovery path: Published messages whose claim has expired (worker crashed)
					(x.State == OutboxMessageState.Published &&
					 x.PublishedAtUtc != null &&
					 x.PublishedAtUtc <= leaseExpiredBeforeUtc)
				))
			// Order Pending/Failed by their due time; order lease-expired Published by when they were claimed
			.OrderBy(x => x.State == OutboxMessageState.Published ? x.PublishedAtUtc : x.NextAttemptAtUtc)
			.ThenBy(x => x.CreatedAtUtc)
			.Take(batchSize)
			.ToListAsync(ct);
	}
}