using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Invite;

public sealed record GetInviteListQueryRequest(
	Guid TenantId,
	string? Search,
	InviteStatus? Status,
	InviteChannel? Channel,
	int Page = 1,
	int PageSize = 20);