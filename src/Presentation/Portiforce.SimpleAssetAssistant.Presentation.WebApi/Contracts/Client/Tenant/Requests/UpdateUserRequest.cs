using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Client.Tenant.Requests;

public sealed record UpdateUserRequest(Role Role, AccountState State, AccountTier Tier, string Alias);
