namespace Portiforce.SAA.Application.Models.Common.DataAccess;

public record PageRequest
{
	public int PageNumber { get; init; } = 1;
	public int PageSize { get; init; } = 20;

	public string? SortBy { get; init; }
	public bool IsDescending { get; init; }

	public PageRequest(int pageNumber, int pageSize, string? sortBy = null, bool isDescending = false)
	{
		PageNumber = pageNumber < 1 ? 1 : pageNumber;
		PageSize = pageSize > 100 ? 100 : (pageSize < 1 ? 20 : pageSize);
		SortBy = sortBy;
		IsDescending = isDescending;
	}
}
