using Portiforce.SAA.Application.Interfaces.Resolvers;
using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.Entitlements.Resolvers;

public sealed class DefaultTenantEntitlementsResolver : ITenantEntitlementsResolver
{
	// MaxPendingInvites: usually 2x MaxActiveUsers, but for demo we want to allow more invites to let users try the product with more people

	public TenantEntitlements Resolve(TenantPlan plan) => plan switch
	{
		TenantPlan.Demo => new TenantEntitlements(
			25,
			60,
			20,
			100,
			5_000,
			false,
			false),

		TenantPlan.Basic => new TenantEntitlements(
			50,
			100,
			10,
			200,
			10_000,
			true,
			false),

		TenantPlan.Advanced => new TenantEntitlements(
			100,
			200,
			20,
			500,
			12_000,
			true,
			false),

		TenantPlan.Pro => new TenantEntitlements(
			200,
			500,
			50,
			1000,
			20_000,
			true,
			true),

		TenantPlan.Custom => new TenantEntitlements(
			25,
			50,
			50,
			1000,
			20_000,
			true,
			true),

		_ => throw new ArgumentOutOfRangeException(nameof(plan))
	};
}