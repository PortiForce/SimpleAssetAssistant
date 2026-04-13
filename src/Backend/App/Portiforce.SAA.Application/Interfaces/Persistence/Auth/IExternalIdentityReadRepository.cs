using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IExternalIdentityReadRepository
{
	Task<ExternalIdentityDetails?> FindGoogleIdentityAsync(string googleUserExternalId, CancellationToken ct);

	Task<bool> ExistsAsync(
		TenantId tenantId,
		AuthProvider provider,
		string providerSubject,
		CancellationToken ct);
}