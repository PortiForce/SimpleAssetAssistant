using Portiforce.SAA.Application.Interfaces.Models.Tenant;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;

public sealed record TenantSummary(
	TenantId Id,
	string Name,
	string Code,
	string DomainPrefix,
	TenantPlan Plan,
	TenantState State
) : ITenantInfo;
