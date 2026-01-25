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
		var tenant = await db.Tenants
			.AsNoTracking()
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

		if (tenant is null)
		{
			return null;
		}

		return new TenantDetails(
			TenantId: tenant.Id,
			Name: tenant.Name,
			Code: tenant.Code,
			BrandName: tenant.BrandName,
			DomainPrefix: tenant.DomainPrefix,
			Plan: tenant.Plan,
			State: tenant.State,
			RestrictedAssets: new ReadOnlySet<AssetId>(new HashSet<AssetId>(tenant.RestrictedAssets)),
			RestrictedPlatforms: new ReadOnlySet<PlatformId>(new HashSet<PlatformId>(tenant.RestrictedPlatforms))
		);
	}
}