using Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;

public sealed record AccountSummary(
	TenantId TenantId,
	AccountId Id,
	string Email,
	AccountState State,
	Role Role) : IDetailsProjection, IAccountInfo;
