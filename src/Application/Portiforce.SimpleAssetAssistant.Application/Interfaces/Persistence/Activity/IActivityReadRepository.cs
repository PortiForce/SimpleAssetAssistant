using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;

public interface IActivityReadRepository : IReadRepository<ActivityDetails, ActivityId>
{
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
