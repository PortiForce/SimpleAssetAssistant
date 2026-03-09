using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Account;

public sealed record AccountListItemResponse(
	Guid Id,
	Guid TenantId,
	string Alias,
	UiAccountTier AccountTier,
	UiAccountState AccountState);
