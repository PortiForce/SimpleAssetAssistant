using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Profile.Account;

public record AccountListItemDto
{
	public required AccountId Id { get; init; }

	public required string Alias { get; init; }

	public required string Email { get; init; }

	public required AccountTier Tier { get; init; }

	public required AccountState State { get; init; }
}
