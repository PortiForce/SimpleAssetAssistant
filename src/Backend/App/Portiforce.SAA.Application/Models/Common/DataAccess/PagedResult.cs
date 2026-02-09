namespace Portiforce.SAA.Application.Models.Common.DataAccess;

public record PagedResult<T>(
	IReadOnlyList<T> Items,
	int TotalCount,
	int PageNumber,
	int PageSize
)
{
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
	public bool HasNextPage => PageNumber < TotalPages;
	public bool HasPreviousPage => PageNumber > 1;
}
