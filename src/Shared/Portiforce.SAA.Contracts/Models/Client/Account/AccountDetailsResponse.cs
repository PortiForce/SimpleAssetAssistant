using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Account;

public sealed record AccountDetailsResponse(
	Guid Id,
	Guid TenantId,
	string Alias,
	UiAccountTier AccountTier,
	UiAccountState AccountState);
