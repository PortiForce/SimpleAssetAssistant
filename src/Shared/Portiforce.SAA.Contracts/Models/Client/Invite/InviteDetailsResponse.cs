using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record InviteDetailsResponse(
	Guid Id,
	Guid TenantId,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	InviteAccountTier InviteTier,
	InviteTenantRole InviteRole,
	InviteStatus State,
	DateTimeOffset CreatedAtUtc,
	DateTimeOffset ExpiresAtUtc,
	Guid InvitedBy,
	int SendTimesCount,
	DateTimeOffset? AcceptedAtUtc,
	Guid? RelatedAccountId,
	bool CanResend,
	bool CanRevoke);
