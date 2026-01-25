using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

internal static class QueryablePagingExtensions
{
	public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
		this IQueryable<T> query,
		PageRequest page,
		CancellationToken ct)
	{
		var total = await query.CountAsync(ct).ConfigureAwait(false);

		var items = await query
			.Skip((page.PageNumber - 1) * page.PageSize)
			.Take(page.PageSize)
			.ToListAsync(ct)
			.ConfigureAwait(false);

		return new PagedResult<T>(
			items,
			total,
			page.PageNumber,
			page.PageSize);
	}

	public static IQueryable<T> ApplyOrderBy<T>(
		this IQueryable<T> query,
		PageRequest page,
		IReadOnlyDictionary<string, Expression<Func<T, object>>> allowedSort)
	{
		if (string.IsNullOrWhiteSpace(page.SortBy))
		{
			// caller should add deterministic fallback order
			return query;
		}

		if (!allowedSort.TryGetValue(page.SortBy, out var expr))
		{
			// ignore unknown sort keys
			return query;
		}

		return page.IsDescending 
			? query.OrderByDescending(expr)
			: query.OrderBy(expr);
	}
}
