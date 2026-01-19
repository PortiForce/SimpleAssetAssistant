using System.Collections.ObjectModel;

using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;

public sealed record TenantDetails(
	TenantId TenantId,
	string Name,
	TenantPlan Plan,
	TenantState State,
	ReadOnlySet<AssetId> RestrictedAssets) : IDetailsProjection;
