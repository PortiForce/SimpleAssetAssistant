namespace Portiforce.SAA.Contracts.Models.Invite;

public sealed record InviteListResponse(
	IReadOnlyList<InviteListItemResponse> Items,
	int TotalCount,
	int PageNumber,
	int PageSize);
