using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Invite;

internal sealed class InviteReadRepository(AssetAssistantDbContext db) : IInviteReadRepository
{
	// Reusable projection to avoiding code duplication
	private static Expression<Func<TenantInvite, InviteListItem>> ListItemSelector =>
		x => new InviteListItem(
			x.Id,
			x.TenantId,
			x.InviteTarget.Value,
			x.InviteTarget.Type,
			x.IntendedTier,
			x.IntendedRole,
			x.State,
			x.CreatedAtUtc,
			x.ExpiresAtUtc,
			x.InvitedByAccountId,
			x.AcceptedAtUtc,
			x.AcceptedAccountId
		);

	private static Expression<Func<TenantInvite, InviteDetails>> DetailsSelector =>
		x => new InviteDetails(
			x.Id,
			x.TenantId,
			x.InviteTarget.Value,
			x.InviteTarget.Type,
			x.IntendedTier,
			x.IntendedRole,
			x.State,
			x.CreatedAtUtc,
			x.ExpiresAtUtc,
			x.InvitedByAccountId,
			x.SendCount,
			x.AcceptedAtUtc,
			x.AcceptedAccountId);

	public async Task<InviteDetails?> GetByIdAsync(Guid id, CancellationToken ct)
	{
		var data = await db.Invites
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(DetailsSelector)
			.SingleOrDefaultAsync(ct);

		return data;
	}

	public async Task<PagedResult<InviteListItem>> GetByTenantIdAsync(TenantId tenantId, PageRequest pageRequest, CancellationToken ct)
	{
		IQueryable<TenantInvite> query = db.Invites
			.AsNoTracking()
			.Where(x => x.TenantId == tenantId);

		int totalCount = await query.CountAsync(ct);

		List<InviteListItem> items = await query
			.OrderBy(x => x.Id)
			.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
			.Take(pageRequest.PageSize)
			.Select(ListItemSelector)
			.ToListAsync(ct);

		return new PagedResult<InviteListItem>(
			items,
			totalCount,
			pageRequest.PageNumber,
			pageRequest.PageSize);
	}

	public async Task<InviteDetails?> GetByInviteTargetAndTenantAsync(InviteTarget inviteTarget, TenantId requestTenantId, CancellationToken ct)
	{
		var data = await db.Invites
			.AsNoTracking()
			.Where(x =>
				x.InviteTarget == inviteTarget &&
				x.TenantId == requestTenantId)
			.Select(DetailsSelector)
			.SingleOrDefaultAsync(ct);

		return data;
	}

	public async Task<List<InviteListItem>> GetByInviteTargetAsync(InviteTarget inviteTarget, CancellationToken ct)
	{
		return await db.Invites
			.AsNoTracking()
			.Where(x => x.InviteTarget == inviteTarget)
			.Select(ListItemSelector)
			.ToListAsync(ct);
	}

	public async Task<TenantInvite?> GetByTenantAndTokenHashAsync(TenantId requestTenantId, byte[] tokenHash, CancellationToken ct)
	{
		var data = await db.Invites
			.AsNoTracking()
			.Where(x =>
				x.TokenHash == tokenHash &&
				x.TenantId == requestTenantId)
			.SingleOrDefaultAsync(ct);

		return data;
	}

	public async Task<int> GetPendingInviteCountAsync(TenantId tenantId, CancellationToken ct)
	{
		return await db.Invites
			.AsNoTracking()
			.Where(x => x.TenantId == tenantId && x.State == InviteState.Created)
			.CountAsync(ct);
	}
}
