using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Activity.Projections;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Activities;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Activity;

public interface IActivityReadRepository : IReadRepository<ActivityDetails, ActivityId>
{
	Task<bool> ExistsByExternalIdAsync(
		string externalId,
		AssetActivityKind activityKind,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		CancellationToken ct);

	Task<bool> ExistsByFingerprintAsync(
		string fingerprint,
		AssetActivityKind activityKind,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		CancellationToken ct);

	public ValueTask<ActivityDetails?> GetDetailsAsync(
		ActivityId id,
		TenantId tenantId,
		AccountId accountId,
		CancellationToken ct);
	
	// Essential for the "Drill-down" requirement
	Task<PagedResult<ActivityListItem>> GetByAccountAsync(
		AccountId accountId,
		PageRequest pageRequest,
		CancellationToken ct);
	
	IAsyncEnumerable<ActivityListItem> GetStreamByAccountAsync(
		AccountId accountId,
		CancellationToken ct);

	/// <summary>
	/// Returns facts ordered by OccurredAt then Id
	/// </summary>
	/// <param name="accountId"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	IAsyncEnumerable<AssetActivityBase> StreamFactsByAccountAsync(AccountId accountId, CancellationToken ct);
}
