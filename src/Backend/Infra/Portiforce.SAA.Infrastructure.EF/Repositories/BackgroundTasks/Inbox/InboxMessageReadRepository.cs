using Microsoft.EntityFrameworkCore;

using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.BackgroundTasks.Inbox;

internal sealed class InboxMessageReadRepository(AssetAssistantDbContext db) : IInboxMessageReadRepository
{
	public async Task<InboxMessage?> GetByIdAsync(Guid id, CancellationToken ct) =>
		await db.InboxMessages
			.SingleOrDefaultAsync(x => x.Id == id, ct);

	public async Task<InboxMessage?> GetByIdAsync(
		TenantId tenantId,
		Guid id,
		CancellationToken ct) =>
		await db.InboxMessages
			.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct);

	public async Task<PagedResult<InboxMessage>> GetListAsync(
		TenantId tenantId,
		HashSet<InboxMessageState>? states,
		string? search,
		PageRequest pageRequest,
		CancellationToken ct)
	{
		IQueryable<InboxMessage> query = db.InboxMessages
			.Where(x => x.TenantId == tenantId);

		if (states is { Count: > 0 })
		{
			query = query.Where(x => states.Contains(x.State));
		}

		if (!string.IsNullOrWhiteSpace(search))
		{
			string normalizedSearch = search.Trim();

			query = query.Where(x =>
				x.Type.Contains(normalizedSearch) ||
				x.PublicReference.Contains(normalizedSearch) ||
				x.Source.Contains(normalizedSearch) ||
				x.RequestPath.Contains(normalizedSearch) ||
				x.HttpMethod.Contains(normalizedSearch) ||
				x.IdempotencyKey.Contains(normalizedSearch));
		}

		int totalCount = await query.CountAsync(ct);

		query = pageRequest.SortBy?.Trim() switch
		{
			nameof(InboxMessage.Type) => pageRequest.IsDescending
				? query.OrderByDescending(x => x.Type)
				: query.OrderBy(x => x.Type),
			nameof(InboxMessage.Source) => pageRequest.IsDescending
				? query.OrderByDescending(x => x.Source)
				: query.OrderBy(x => x.Source),
			nameof(InboxMessage.State) => pageRequest.IsDescending
				? query.OrderByDescending(x => x.State)
				: query.OrderBy(x => x.State),
			nameof(InboxMessage.NextAttemptAtUtc) => pageRequest.IsDescending
				? query.OrderByDescending(x => x.NextAttemptAtUtc)
				: query.OrderBy(x => x.NextAttemptAtUtc),
			_ => pageRequest.IsDescending
				? query.OrderByDescending(x => x.ReceivedAtUtc)
				: query.OrderBy(x => x.ReceivedAtUtc)
		};

		List<InboxMessage> items = await query
			.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
			.Take(pageRequest.PageSize)
			.ToListAsync(ct);

		return new PagedResult<InboxMessage>(
			items,
			totalCount,
			pageRequest.PageNumber,
			pageRequest.PageSize);
	}

	public async Task<IReadOnlyList<InboxMessage>> GetReadyToProcessAsync(
		string type,
		DateTimeOffset nowUtc,
		int batchSize,
		TimeSpan processingLeaseTimeout,
		CancellationToken ct)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(type);

		if (batchSize <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");
		}

		if (processingLeaseTimeout <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(
				nameof(processingLeaseTimeout),
				"Processing lease timeout must be positive.");
		}

		DateTimeOffset leaseExpiredBeforeUtc = nowUtc - processingLeaseTimeout;

		return await db.InboxMessages
			.Where(x =>
				x.Type == type &&
				(

					// Normal retry path: Pending or Failed messages due for processing
					((x.State == InboxMessageState.Received || x.State == InboxMessageState.Failed) &&
					 x.NextAttemptAtUtc != null &&
					 x.NextAttemptAtUtc <= nowUtc) ||

					// Lease recovery path: Processing messages whose claim has expired (worker crashed)
					(x.State == InboxMessageState.Processing &&
					 x.ProcessingStartedAtUtc != null &&
					 x.ProcessingStartedAtUtc <= leaseExpiredBeforeUtc)
				))

			// Order Received/Failed by their due time; order lease-expired Processing by when they were claimed.
			.OrderBy(x => x.State == InboxMessageState.Processing ? x.ProcessingStartedAtUtc : x.NextAttemptAtUtc)
			.ThenBy(x => x.ReceivedAtUtc)
			.Take(batchSize)
			.ToListAsync(ct);
	}
}
