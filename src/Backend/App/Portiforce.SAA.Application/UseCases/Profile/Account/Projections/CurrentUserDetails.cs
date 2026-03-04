using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Projections;

public sealed record CurrentUserDetails(
	TenantId TenantId,
	AccountId Id,
	string Alias,
	string Email,
	AccountTier Tier,
	Role Role,
	AccountState State,
	ContactInfo ContactInfo) : IDetailsProjection, IAccountInfo;