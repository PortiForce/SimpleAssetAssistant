using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Invite;

public interface IInviteReadRepository : IReadRepository<InviteDetailsRaw, Guid>
{
	Task<PagedResult<InviteListItemRaw>> GetListAsync(
		TenantId requestTenantId,
		InviteChannel? requestChannel,
		InviteState? requestState,
		string? requestSearch,
		bool? hasAccount,
		PageRequest requestPageRequest,
		CancellationToken ct);

	Task<InviteDetailsRaw?> GetByInviteTargetAndTenantAsync(
		InviteTarget inviteTarget,
		TenantId requestTenantId,
		CancellationToken ct);

	/// <summary>
	/// Finds all invites across all tenants that share this email
	/// </summary>
	/// <param name="inviteTarget">invite target</param>
	/// <param name="ct"></param>
	/// <returns>list of invites that match provided email address</returns>
	Task<List<InviteListItemRaw>> GetByInviteTargetAsync(InviteTarget inviteTarget, CancellationToken ct);

	Task<TenantInvite?> GetByTenantAndTokenHashAsync(
		TenantId requestTenantId,
		byte[] tokenHash,
		CancellationToken ct);

	Task<int> GetPendingInviteCountAsync(TenantId tenantId, CancellationToken ct);
}
