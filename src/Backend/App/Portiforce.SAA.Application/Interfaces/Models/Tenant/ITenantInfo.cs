using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Models.Tenant;

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
