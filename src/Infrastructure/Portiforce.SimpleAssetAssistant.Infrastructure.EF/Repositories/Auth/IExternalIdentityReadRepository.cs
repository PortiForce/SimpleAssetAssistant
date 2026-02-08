using Microsoft.EntityFrameworkCore;

using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;

internal sealed class ExternalIdentityReadRepository(AssetAssistantDbContext db) : IExternalIdentityReadRepository
{
	public async Task<ExternalIdentityDetails?> FindGoogleIdentityAsync(
		string googleUserExternalId,
		CancellationToken ct)
	{
		return await db.ExternalIdentities
			.AsNoTracking()
			.Where(x =>
					x.Provider == AuthProvider.Google &&
					x.ProviderSubject == googleUserExternalId
			)
			.Select(x => new ExternalIdentityDetails(
				x.Id,
				x.TenantId,
				x.AccountId,
				x.Provider,
				x.IsPrimary
			))
			.SingleOrDefaultAsync(ct);
	}
}
