using Microsoft.EntityFrameworkCore;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Auth;

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
