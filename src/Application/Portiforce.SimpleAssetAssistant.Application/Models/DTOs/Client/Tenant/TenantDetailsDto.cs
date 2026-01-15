using System.Collections.ObjectModel;

using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Client.Tenant;

public record TenantDetailsDto
{
	public required TenantId TenantId { get; init; }

	public required string Name { get; init; }

	public required TenantPlan Plan { get; init; }

	public required TenantState State { get; init; }

	public ReadOnlySet<AssetId> RestrictedAssets { get; init; }
}
