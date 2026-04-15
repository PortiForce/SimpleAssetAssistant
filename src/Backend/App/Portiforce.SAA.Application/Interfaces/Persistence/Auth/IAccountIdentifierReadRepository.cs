using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IAccountIdentifierReadRepository
{
	Task<bool> ExistsAsync(
		TenantId tenantId,
		AccountIdentifierKind verificationKind,
		string verificationValue,
		CancellationToken ct);
}