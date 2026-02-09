using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Api.Contracts.Client.Tenant.Requests;

public sealed record UpdateUserRequest(Role Role, AccountState State, AccountTier Tier, string Alias);
