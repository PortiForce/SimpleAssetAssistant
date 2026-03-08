using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.User;

public sealed record GetAccountListQueryRequest(
	string? Search,
	InviteStatus? Status,
	InviteChannel? Channel,
	int Page = 1,
	int PageSize = 20);