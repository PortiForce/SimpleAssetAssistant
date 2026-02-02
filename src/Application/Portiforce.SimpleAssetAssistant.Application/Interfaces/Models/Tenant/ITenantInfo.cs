using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Models.Tenant;

/// <summary>
/// A unifying interface for TenantDetails and TenantListItem
/// to allow custom flows/use cases from both Read Models.
/// </summary>
public interface ITenantInfo
{
	TenantId Id { get; }
	string Code { get; }
	string? DomainPrefix { get; }
	TenantState State { get; }
	TenantPlan Plan { get; }
}
