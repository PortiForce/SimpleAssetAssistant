using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record GetInviteListQueryRequest(
	string? Search,
	InviteStatus? Status,
	InviteChannel? Channel,
	int Page = 1,
	int PageSize = 20);