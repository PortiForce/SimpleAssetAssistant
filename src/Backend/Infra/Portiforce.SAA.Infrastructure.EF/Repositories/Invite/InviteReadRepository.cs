using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Invite;

internal sealed class InviteReadRepository(AssetAssistantDbContext db) : IInviteReadRepository
{
	// Reusable projection to avoiding code duplication
	private static Expression<Func<TenantInvite, InviteListItemRaw>> ListItemSelector =>
		x => new InviteListItemRaw(
			x.Id,
			x.TenantId,
			x.InviteTarget.Value,
			x.InviteTarget.Channel,
			x.InviteTarget.Kind,
			x.IntendedTier,
			x.IntendedRole,
			x.State,
			x.CreatedAtUtc,
			x.ExpiresAtUtc,
			x.InvitedByAccountId,
			x.UpdatedAtUtc,
			x.AcceptedAccountId,
			x.BlockFutureInvites);

	private static Expression<Func<TenantInvite, InviteDetailsRaw>> DetailsSelector =>
		x => new InviteDetailsRaw(
			x.Id,
			x.TenantId,
			x.InviteTarget.Value,
			x.InviteTarget.Channel,
			x.InviteTarget.Kind,
			x.IntendedTier,
			x.IntendedRole,
			x.State,
			x.CreatedAtUtc,
			x.ExpiresAtUtc,
			x.InvitedByAccountId,
			x.SendCount,
			x.UpdatedAtUtc,
			x.AcceptedAccountId,
			x.BlockFutureInvites);

	public async Task<InviteDetailsRaw?> GetByIdAsync(Guid id, CancellationToken ct)
	{
		InviteDetailsRaw? data = await db.Invites
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(DetailsSelector)
			.SingleOrDefaultAsync(ct);

		return data;
	}

	public async Task<PagedResult<InviteListItemRaw>> GetListAsync(
		TenantId tenantId,
		HashSet<InviteChannel>? channels,
		HashSet<InviteState>? states,
		string? search,
		bool? hasAccount,
		PageRequest pageRequest,
		CancellationToken ct)
	{
		IQueryable<TenantInvite> query = db.Invites
			.AsNoTracking()
			.Where(x => x.TenantId == tenantId);

		if (channels is { Count: > 0 })
		{
			query = query.Where(x => channels.Contains(x.InviteTarget.Channel));
		}

		if (states is { Count: > 0 })
		{
			query = query.Where(x => states.Contains(x.State));
		}

		if (!string.IsNullOrWhiteSpace(search))
		{
			query = query.Where(x => x.InviteTarget.Value.Contains(search));
		}

		if (hasAccount.HasValue)
		{
			query = hasAccount.Value
				? query.Where(x => x.AcceptedAccountId != AccountId.Empty && x.AcceptedAccountId != null)
				: query.Where(x => x.AcceptedAccountId == null || x.AcceptedAccountId == AccountId.Empty);
		}

		int skip = (pageRequest.PageNumber - 1) * pageRequest.PageSize;
		int totalCount = await query.CountAsync(ct);

		List<InviteListItemRaw> items = await query
			.OrderBy(x => x.Id)
			.Skip(skip)
			.Take(pageRequest.PageSize)
			.Select(ListItemSelector)
			.ToListAsync(ct);

		return new PagedResult<InviteListItemRaw>(
			items,
			totalCount,
			pageRequest.PageNumber,
			pageRequest.PageSize);
	}

	public async Task<InviteDetailsRaw?> GetByInviteTargetAndTenantAsync(
		InviteTarget inviteTarget,
		TenantId requestTenantId,
		CancellationToken ct)
	{
		InviteDetailsRaw? data = await db.Invites
			.AsNoTracking()
			.Where(x =>
				x.InviteTarget == inviteTarget &&
				x.TenantId == requestTenantId)
			.Select(DetailsSelector)
			.SingleOrDefaultAsync(ct);

		return data;
	}

	public async Task<List<InviteListItemRaw>> GetByInviteTargetAsync(InviteTarget inviteTarget, CancellationToken ct)
	{
		return await db.Invites
			.AsNoTracking()
			.Where(x => x.InviteTarget == inviteTarget)
			.Select(ListItemSelector)
			.ToListAsync(ct);
	}

	public async Task<TenantInvite?> GetByTenantAndTokenHashAsync(
		TenantId requestTenantId,
		byte[] tokenHash,
		CancellationToken ct)
	{
		TenantInvite? data = await db.Invites
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

	public async Task<PagedResult<InviteListItemRaw>> GetByTenantIdAsync(
		TenantId tenantId,
		PageRequest pageRequest,
		CancellationToken ct)
	{
		IQueryable<TenantInvite> query = db.Invites
			.AsNoTracking()
			.Where(x => x.TenantId == tenantId);

		int totalCount = await query.CountAsync(ct);

		List<InviteListItemRaw> items = await query
			.OrderBy(x => x.Id)
			.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
			.Take(pageRequest.PageSize)
			.Select(ListItemSelector)
			.ToListAsync(ct);

		return new PagedResult<InviteListItemRaw>(
			items,
			totalCount,
			pageRequest.PageNumber,
			pageRequest.PageSize);
	}
}