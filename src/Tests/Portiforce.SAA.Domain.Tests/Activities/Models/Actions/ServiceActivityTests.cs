using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

public sealed class ServiceActivityTests
{
	[Fact]
	public void Create_WhenPrincipalLegIsInflow_ShouldThrow()
	{
		var legs = new[]
		{
			ActivityTestFactory.PrincipalLeg(MovementDirection.Inflow)
		};

		var act = () => ServiceActivity.Create(
			tenantId: TenantId.New(),
			platformAccountId: PlatformAccountId.New(),
			occurredAt: DateTimeOffset.UtcNow,
			reason: AssetActivityReason.ServiceFee,
			legs: legs,
			externalMetadata: ActivityTestFactory.ExternalMetadata(),
			serviceType: ServiceType.Custody,
			id: null);

		act.Should().Throw<DomainValidationException>()
			.WithMessage("*only Outflow principal legs*");
	}
}