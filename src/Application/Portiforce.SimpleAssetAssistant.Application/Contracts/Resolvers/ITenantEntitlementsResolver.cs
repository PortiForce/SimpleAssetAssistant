using Portiforce.SimpleAssetAssistant.Application.Entitlements;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

namespace Portiforce.SimpleAssetAssistant.Application.Contracts.Resolvers;

public interface ITenantEntitlementsResolver
{
	TenantEntitlements Resolve(TenantPlan plan);
}
