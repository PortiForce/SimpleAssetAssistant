using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Projections;

public sealed record AccountIdentifierDetails(
	AccountIdentifierId Id,
	TenantId TenantId,
	AccountId AccountId,
	AccountIdentifierKind Kind,
	string Value,
	bool IsVerified,
	bool IsPrimary);