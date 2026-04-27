using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;

public interface IInboxMessageReadRepository : IReadRepository<InboxMessage, Guid>
{
	Task<InboxMessage?> GetByIdAsync(
		TenantId tenantId,
		Guid id,
		CancellationToken ct);

	Task<PagedResult<InboxMessage>> GetListAsync(
		TenantId tenantId,
		HashSet<InboxMessageState>? states,
		string? search,
		PageRequest pageRequest,
		CancellationToken ct);

	Task<IReadOnlyList<InboxMessage>> GetReadyToProcessAsync(
		string type,
		DateTimeOffset nowUtc,
		int batchSize,
		TimeSpan processingLeaseTimeout,
		CancellationToken ct);
}