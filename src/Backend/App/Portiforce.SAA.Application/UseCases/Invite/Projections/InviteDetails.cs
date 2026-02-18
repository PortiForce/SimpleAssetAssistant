using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Projections;

public sealed record InviteDetails(
	Guid Id,
	TenantId TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	InviteAccountTier InviteTier,
	InviteTenantRole InviteRole,
	InviteState State,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	AccountId InvitedBy,
	int SendTimesCount,
	DateTimeOffset? AcceptedAtUtc,
	AccountId? RelatedAccountId
) : IDetailsProjection;
