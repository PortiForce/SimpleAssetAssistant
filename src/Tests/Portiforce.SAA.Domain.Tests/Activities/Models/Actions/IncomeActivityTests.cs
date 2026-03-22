using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

public sealed class IncomeActivityTests
{
	[Fact]
	public void Create_WhenPrincipalLegIsOutflow_ShouldThrow()
	{
		var legs = new[]
		{
			ActivityTestFactory.PrincipalLeg(MovementDirection.Outflow)
		};

		var act = () => IncomeActivity.Create(
			tenantId: TenantId.New(),
			platformAccountId: PlatformAccountId.New(),
			occurredAt: DateTimeOffset.UtcNow,
			reason: AssetActivityReason.Reward,
			legs: legs,
			externalMetadata: ActivityTestFactory.ExternalMetadata(),
			id: null);

		act.Should().Throw<DomainValidationException>()
			.WithMessage("*only Inflow principal legs*");
	}
}