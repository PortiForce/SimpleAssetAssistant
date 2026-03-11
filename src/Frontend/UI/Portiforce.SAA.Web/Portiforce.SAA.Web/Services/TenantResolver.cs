using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using Portiforce.SAA.Contracts.Models.Client;
using Portiforce.SAA.Contracts.Services;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Web.Services;

public sealed class TenantResolver : ITenantResolver
{
	private readonly AssetAssistantDbContext _db;
	private readonly IMemoryCache _cache;
	private readonly ILogger<TenantResolver> _logger;

	private sealed record TenantCacheItem(Guid TenantId, string Prefix, string Name);

	public TenantResolver(
		AssetAssistantDbContext db,
		IMemoryCache cache,
		ILogger<TenantResolver> logger)
	{
		_db = db;
		_cache = cache;
		_logger = logger;
	}

	public async Task<TenantResolution?> ResolveByPrefixAsync(string prefix, CancellationToken ct)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(prefix))
			{
				return null;
			}

			var cacheKey = $"tenant:{prefix.ToLowerInvariant()}";

			var item = await _cache.GetOrCreateAsync(cacheKey, async entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
				entry.SlidingExpiration = TimeSpan.FromMinutes(2);

				var tenant = await _db.Tenants
					.AsNoTracking()
					.Where(t => t.DomainPrefix == prefix &&
					            (t.State == TenantState.Active || t.State == TenantState.Suspended))
					.Select(t => new { t.Id, t.DomainPrefix, t.BrandName })
					.SingleOrDefaultAsync(ct);

				return tenant is null
					? null
					: new TenantCacheItem(tenant.Id.Value, tenant.DomainPrefix, tenant.BrandName);
			});

			return item is null ? null : new TenantResolution(item.TenantId, item.Prefix, item.Name);
		}
		catch (SqlException ex)
		{
			_logger.LogError(ex,
				"Failed to resolve tenant by prefix {Prefix}. Database unavailable.",
				prefix);

			return null; 
		}
		catch (TimeoutException ex)
		{
			_logger.LogError(ex,
				"Timeout while resolving tenant by prefix {Prefix}.",
				prefix);

			return null;
		}
	}
}