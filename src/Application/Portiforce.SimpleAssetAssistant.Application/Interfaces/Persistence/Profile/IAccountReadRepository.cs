using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;

public interface IAccountReadRepository : IReadRepository<AccountDetails, AccountId>
{
	Task<PagedResult<AccountListItem>> GetByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct);

	Task<AccountDetails?> GetByEmailAndTenantAsync(
		Email googleUserEmail,
		TenantId requestTenantId,
		CancellationToken ct);

	/// <summary>
	/// Finds all accounts across all tenants that share this email
	/// </summary>
	/// <param name="email">email to check</param>
	/// <param name="ct"></param>
	/// <returns>list of accounts that match provided email address</returns>
	Task<List<AccountListItem>> GetByEmailAsync(Email email, CancellationToken ct);

	/// <summary>
	/// Returns number of users where State == Active
	/// </summary>
	/// <param name="tenantId">related tenantId</param>
	/// <param name="ct"></param>
	/// <returns></returns>
	Task<int> GetActiveUserCountAsync(TenantId tenantId, CancellationToken ct);
}
