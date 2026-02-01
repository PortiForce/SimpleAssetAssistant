using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Client;

internal sealed class TenantReadRepository(AssetAssistantDbContext db) : ITenantReadRepository
{
	public async Task<TenantDetails?> GetByIdAsync(TenantId id, CancellationToken ct)
	{
		// AsSplitQuery: Prevents data duplication (Cartesian explosion) when loading multiple collections.
		// RestrictedAssets, RestrictedPlatforms: EF Core handles these as separate efficient SQL queries now
		var tenantData = await db.Tenants
			.AsNoTracking()
			.AsSplitQuery()
			.Where(x => x.Id == id)
			.Select(x => new
			{
				x.Id,
				x.Name,
				x.Code,
				x.BrandName,
				x.DomainPrefix,
				x.Plan,
				x.State,
				RestrictedAssets = x.RestrictedAssets.Select(r => r.AssetId).ToList(),
				RestrictedPlatforms = x.RestrictedPlatforms.Select(r => r.PlatformId).ToList()
			})
			.SingleOrDefaultAsync(ct);

		if (tenantData is null)
		{
			return null;
		}

		// Mapping logic remains here because 'ReadOnlySet' is complex to construct inside LINQ
		return new TenantDetails(
			TenantId: tenantData.Id,
			Name: tenantData.Name,
			Code: tenantData.Code,
			BrandName: tenantData.BrandName,
			DomainPrefix: tenantData.DomainPrefix,
			Plan: tenantData.Plan,
			State: tenantData.State,
			RestrictedAssets: new ReadOnlySet<AssetId>(new HashSet<AssetId>(tenantData.RestrictedAssets)),
			RestrictedPlatforms: new ReadOnlySet<PlatformId>(new HashSet<PlatformId>(tenantData.RestrictedPlatforms))
		);
	}

	public async Task<TenantSummary?> GetSummaryByIdAsync(TenantId id, CancellationToken ct)
	{
		// Direct Projection: Projects directly to the DTO, skipping the intermediate anonymous object.
		return await db.Tenants
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(x => new TenantSummary(
				x.Id,
				x.Name,
				x.DomainPrefix,
				x.Plan,
				x.State
			))
			.SingleOrDefaultAsync(ct);
	}
}