using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Client.Tenant;

public record TenantListItemDto
{
	public required TenantId Id { get; init; }

	public required string Name { get; init; }

	public required TenantPlan Plan { get; init; }

	public required TenantState State { get; init; }
}
