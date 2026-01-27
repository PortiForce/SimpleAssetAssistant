using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Profile;

internal sealed class AccountReadRepository : IAccountReadRepository
{
	public Task<AccountDetails?> GetByIdAsync(AccountId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<AccountListItem>> GetByTenantIdAsync(TenantId tenantId, PageRequest pageRequest, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<AccountDetails?> GetByEmailAndTenantAsync(string googleUserEmail, TenantId requestTenantId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
