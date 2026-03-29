using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IAccountIdentifierReadRepository
{
	Task<AccountIdentifierDetails?> FindGoogleIdentityAsync(string googleUserExternalId, CancellationToken ct);

	Task<bool> ExistsAsync(
		TenantId tenantId,
		AccountIdentifierKind verificationKind,
		string verificationValue,
		CancellationToken ct);
}