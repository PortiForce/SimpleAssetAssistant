using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Projections;

public sealed record InviteDetails(
	Guid Id,
	TenantId TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	InviteTargetKind InviteTargetKind,
	AccountTier InviteTier,
	Role InviteRole,
	InviteState State,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	AccountId InvitedBy,
	int SendTimesCount,
	DateTimeOffset? AcceptedAtUtc,
	AccountId? RelatedAccountId,
	bool? BlockFutureInvitesForTarget,
	bool CanResend,
	bool CanRevoke);

public sealed record InviteDetailsRaw(
	Guid Id,
	TenantId TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	InviteTargetKind InviteTargetKind,
	AccountTier InviteTier,
	Role InviteRole,
	InviteState State,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	AccountId InvitedBy,
	int SendTimesCount,
	DateTimeOffset? AcceptedAtUtc,
	AccountId? RelatedAccountId,
	bool? BlockFutureInvitesForTarget) : IDetailsProjection;