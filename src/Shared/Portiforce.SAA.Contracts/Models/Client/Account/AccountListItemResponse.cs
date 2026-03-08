using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.User;

public sealed record AccountListItemResponse(
	Guid Id,
	Guid TenantId,
	string Alias,
	InviteChannel InviteChannel,
	InviteAccountTier InviteTier,
	InviteTenantRole InviteRole,
	InviteStatus Status);
