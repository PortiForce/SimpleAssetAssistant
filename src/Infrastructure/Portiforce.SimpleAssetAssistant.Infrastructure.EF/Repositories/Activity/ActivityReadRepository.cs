using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Activity;

internal sealed class ActivityReadRepository : IActivityReadRepository
{
	public Task<ActivityDetails?> GetByIdAsync(ActivityId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<bool> ExistsByExternalIdAsync(string externalId, AssetActivityKind activityKind, TenantId tenantId,
		PlatformAccountId platformAccountId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<bool> ExistsByFingerprintAsync(string fingerprint, AssetActivityKind activityKind, TenantId tenantId,
		PlatformAccountId platformAccountId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<ActivityListItem>> GetByAccountAsync(AccountId accountId, PageRequest pageRequest, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<ActivityListItem> GetStreamByAccountAsync(AccountId accountId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<AssetActivityBase> StreamFactsByAccountAsync(AccountId accountId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
