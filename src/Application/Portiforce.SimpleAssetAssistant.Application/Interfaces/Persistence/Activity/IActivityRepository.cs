using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;

public interface IActivityRepository
{
	// Essential for the "Drill-down" requirement
	Task<PagedResult<AssetActivityBase>> GetByAccountAsync(
		AccountId accountId,
		PageRequest pageRequest,
		CancellationToken ct);

	// Essential for "Rebuild Balances"
	// Note: No paging, we need ALL history to rebuild state.
	// Likely returns IAsyncEnumerable or a stream for performance.
	IAsyncEnumerable<AssetActivityBase> GetStreamByAccountAsync(
		AccountId accountId,
		CancellationToken ct);
}
