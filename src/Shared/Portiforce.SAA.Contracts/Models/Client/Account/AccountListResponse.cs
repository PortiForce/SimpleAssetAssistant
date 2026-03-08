namespace Portiforce.SAA.Contracts.Models.Client.User;

public sealed record AccountListResponse(
	IReadOnlyList<AccountListItemResponse> Items,
	int TotalCount,
	int PageNumber,
	int PageSize);
