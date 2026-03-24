using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

public sealed class ServiceActivityTests
{
	[Fact]
	public void Create_WhenPrincipalLegIsInflow_ShouldThrow()
	{
		AssetMovementLeg[] legs = [ActivityTestFactory.PrincipalLeg(MovementDirection.Inflow)];

		Func<ServiceActivity> act = () => ServiceActivity.Create(
			TenantId.New(),
			PlatformAccountId.New(),
			DateTimeOffset.UtcNow,
			AssetActivityReason.ServiceFee,
			legs,
			ActivityTestFactory.ExternalMetadata());

		_ = act.Should().Throw<DomainValidationException>()
			.WithMessage("*only Outflow principal legs*");
	}
}