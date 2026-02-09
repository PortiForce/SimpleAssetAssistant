using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Activity.Projections;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Activities;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Activity;

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

	public ValueTask<ActivityDetails?> GetDetailsAsync(ActivityId id, TenantId tenantId, AccountId accountId, CancellationToken ct)
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
