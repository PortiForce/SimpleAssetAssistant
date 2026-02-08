using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.Auth;

public interface ICurrentUser
{
	AccountId Id { get; }

	TenantId TenantId { get; }

	bool IsAuthenticated { get; }

	Role Role { get; }

	AccountState State { get; }
}
