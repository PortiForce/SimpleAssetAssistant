using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Models.Auth;

public interface ICurrentUser
{
	AccountId Id { get; }

	TenantId TenantId { get; }

	bool IsAuthenticated { get; }

	Role Role { get; }

	AccountState State { get; }
}
