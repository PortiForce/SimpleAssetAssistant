using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Projections;

public sealed record InviteListItem(
	Guid Id,
	TenantId TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	AccountTier InviteTier,
	Role InviteRole,
	InviteState State,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	AccountId InvitedBy,
	DateTimeOffset? AcceptedAtUtc,
	AccountId? RelatedAccountId,
	bool CanResend,
	bool CanRevoke);

public sealed record InviteListItemRaw(
	Guid Id,
	TenantId TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	AccountTier InviteTier,
	Role InviteRole,
	InviteState State,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	AccountId InvitedBy,
	DateTimeOffset? AcceptedAtUtc,
	AccountId? RelatedAccountId) : IListItemProjection;
