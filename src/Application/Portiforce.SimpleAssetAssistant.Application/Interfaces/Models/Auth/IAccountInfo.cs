using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Auth;

/// <summary>
/// A unifying interface for AccountDetails and AccountListItem
/// to allow Token Generation from both Read Models.
/// </summary>
public interface IAccountInfo
{
	AccountId Id { get; }
	TenantId TenantId { get; }
	string Email { get; }
	AccountState State { get; }
	Role Role { get; }
}
