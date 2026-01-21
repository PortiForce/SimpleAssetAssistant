using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;

public sealed record TenantListItem(
	TenantId Id,
	string Name,
	string Code,
	string? BrandName,
	string? DomainPrefix,
	TenantPlan Plan,
	TenantState State) : IListItemProjection;
