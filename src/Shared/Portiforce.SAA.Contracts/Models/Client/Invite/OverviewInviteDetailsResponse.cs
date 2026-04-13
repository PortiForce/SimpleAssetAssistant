using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record OverviewInviteDetailsResponse(
	Guid Id,
	string InviteTargetValue,
	InviteChannel InviteChannel,
	InviteAccountTier InviteTier,
	InviteTenantRole InviteRole,
	InviteStatus State,
	DateTimeOffset ExpiresAtUtc,
	int SendTimesCount,
	DateTimeOffset? AcceptedAtUtc,
	bool CanAccept,
	bool CanDecline,
	InviteOverviewViewMode ViewMode,
	bool HasActiveSession,
	string? ActiveSessionDisplayName);