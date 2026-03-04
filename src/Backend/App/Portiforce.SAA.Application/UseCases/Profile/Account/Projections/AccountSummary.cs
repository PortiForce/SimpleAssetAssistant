using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Projections;

public sealed record AccountSummary(
	TenantId TenantId,
	AccountId Id,
	string Email,
	AccountState State,
	Role Role) : IDetailsProjection, IAccountInfo;
