using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth.Models;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;

public sealed record AccountListItem(
	AccountId Id,
	TenantId TenantId,
	string Alias,
	string Email,
	AccountTier Tier,
	Role Role,
	AccountState State) : IListItemProjection, IAccountInfo;
