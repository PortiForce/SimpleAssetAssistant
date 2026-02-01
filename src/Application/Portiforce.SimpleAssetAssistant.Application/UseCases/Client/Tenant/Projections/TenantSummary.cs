using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;

public sealed record TenantSummary(
	TenantId Id,
	string Name,
	string DomainPrefix,
	TenantPlan Plan,
	TenantState State
);
