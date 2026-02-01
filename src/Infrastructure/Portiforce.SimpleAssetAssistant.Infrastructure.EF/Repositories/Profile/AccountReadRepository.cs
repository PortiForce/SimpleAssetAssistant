using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Profile;

internal sealed class AccountReadRepository(AssetAssistantDbContext db) : IAccountReadRepository
{
	// Reusable projection to avoiding code duplication
	private static Expression<Func<Account, AccountListItem>> ListItemSelector =>
		x => new AccountListItem(
			x.Id,
			x.TenantId,
			x.Alias,
			x.Contact.Email.Value,
			x.Tier,
			x.Role,
			x.State
		);

	public async Task<AccountDetails?> GetByIdAsync(AccountId id, CancellationToken ct)
	{
		// using anonymous projection to fetch only needed columns before hydrating the heavy Domain DTO.
		var data = await db.Accounts
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(x => new
			{
				x.TenantId,
				x.Id,
				x.Alias,
				Email = x.Contact.Email.Value,
				x.Tier,
				x.State,
				x.Role,
				x.Contact
			})
			.SingleOrDefaultAsync(ct);

		if (data is null)
		{
			return null;
		}

		return new AccountDetails(
			data.TenantId,
			data.Id,
			data.Alias,
			data.Email,
			data.Tier,
			data.State,
			data.Role,
			data.Contact);
	}

	public async Task<PagedResult<AccountListItem>> GetByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct)
	{
		IQueryable<Account> query = db.Accounts
			.AsNoTracking()
			.Where(x => x.TenantId == tenantId);

		int totalCount = await query.CountAsync(ct);

		List<AccountListItem> items = await query
			.OrderBy(x => x.Alias) 
			.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
			.Take(pageRequest.PageSize)
			.Select(ListItemSelector) 
			.ToListAsync(ct);

		return new PagedResult<AccountListItem>(
			items,
			totalCount,
			pageRequest.PageNumber,
			pageRequest.PageSize);
	}

	public async Task<AccountDetails?> GetByEmailAndTenantAsync(
		string googleUserEmail,
		TenantId requestTenantId,
		CancellationToken ct)
	{
		var data = await db.Accounts
			.AsNoTracking()
			.Where(x =>
				x.Contact.Email.Value == googleUserEmail &&
				x.TenantId == requestTenantId)
			.Select(x => new
			{
				x.TenantId,
				x.Id,
				x.Alias,
				Email = x.Contact.Email.Value,
				x.Tier,
				x.State,
				x.Role,
				x.Contact
			})
			.SingleOrDefaultAsync(ct);

		if (data is null)
		{
			return null;
		}

		return new AccountDetails(
			data.TenantId,
			data.Id,
			data.Alias,
			data.Email,
			data.Tier,
			data.State,
			data.Role,
			data.Contact);
	}

	
	public async Task<List<AccountListItem>> GetByEmailAsync(string email, CancellationToken ct)
	{
		return await db.Accounts
			.AsNoTracking()
			.Where(x => x.Contact.Email.Value == email)
			.Select(ListItemSelector) 
			.ToListAsync(ct);
	}

	public async Task<int> GetActiveUserCountAsync(TenantId tenantId, CancellationToken ct)
	{
		return await db.Accounts
			.AsNoTracking()
			.Where(x => x.TenantId == tenantId && x.State == AccountState.Active)
			.CountAsync(ct);
	}
}