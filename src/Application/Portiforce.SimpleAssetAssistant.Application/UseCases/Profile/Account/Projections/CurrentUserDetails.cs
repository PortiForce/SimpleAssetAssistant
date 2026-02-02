using Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;

public sealed record CurrentUserDetails(
	TenantId TenantId,
	AccountId Id,
	string Alias,
	string Email,
	AccountTier Tier,
	Role Role,
	AccountState State,
	ContactInfo ContactInfo) : IDetailsProjection, IAccountInfo;