using Portiforce.SAA.Application.Entitlements;
using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.Interfaces.Resolvers;

public interface ITenantEntitlementsResolver
{
	TenantEntitlements Resolve(TenantPlan plan);
}
