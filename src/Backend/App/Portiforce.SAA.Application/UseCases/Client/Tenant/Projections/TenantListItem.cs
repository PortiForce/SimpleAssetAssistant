using Portiforce.SAA.Application.Interfaces.Models.Tenant;
using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;

public sealed record TenantListItem(
	TenantId Id,
	string Name,
	string Code,
	string? BrandName,
	string? DomainPrefix,
	TenantPlan Plan,
	TenantState State) : IListItemProjection, ITenantInfo;
