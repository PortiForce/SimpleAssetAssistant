namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record InviteListResponse(
	IReadOnlyList<InviteListItemResponse> Items,
	int TotalCount,
	int PageNumber,
	int PageSize);