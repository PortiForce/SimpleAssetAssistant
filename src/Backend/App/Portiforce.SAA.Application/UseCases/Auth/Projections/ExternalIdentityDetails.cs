using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Projections;

public sealed record ExternalIdentityDetails(
	ExternalIdentityId Id,
	TenantId TenantId,
	AccountId AccountId,
	AuthProvider Provider,
	bool IsPrimary);
