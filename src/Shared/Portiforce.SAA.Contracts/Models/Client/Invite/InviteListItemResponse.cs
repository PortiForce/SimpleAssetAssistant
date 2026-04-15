using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record InviteListItemResponse(
	Guid Id,
	Guid TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	InviteAccountTier InviteTier,
	InviteTenantRole InviteRole,
	InviteStatus Status,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	Guid InvitedBy,
	DateTimeOffset? AcceptedAtUtc,
	Guid? RelatedAccountId,
	bool CanResend,
	bool CanRevoke);