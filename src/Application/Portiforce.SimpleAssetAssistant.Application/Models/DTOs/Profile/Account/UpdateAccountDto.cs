using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Profile.Account;

public  record UpdateAccountDto
{
	public required AccountId Id { get; init; }
	public AccountState State { get; init; }
	public AccountTier Tier { get; init; }
}
