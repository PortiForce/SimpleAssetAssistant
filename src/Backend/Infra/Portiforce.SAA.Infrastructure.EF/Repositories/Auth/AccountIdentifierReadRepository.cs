using Microsoft.EntityFrameworkCore;

using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Auth;

internal sealed class AccountIdentifierReadRepository(AssetAssistantDbContext db) : IAccountIdentifierReadRepository
{
	public async Task<bool> ExistsAsync(
		TenantId tenantId,
		AccountIdentifierKind verificationKind,
		string verificationValue,
		CancellationToken ct)
	{
		return await db.AccountIdentifiers
			.AsNoTracking()
			.AnyAsync(
				x => x.TenantId == tenantId &&
					 x.Kind == verificationKind &&
					 x.Value == verificationValue,
				ct);
	}
}