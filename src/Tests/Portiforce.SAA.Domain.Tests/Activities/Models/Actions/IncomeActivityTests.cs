using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

public sealed class IncomeActivityTests
{
	[Fact]
	public void Create_WhenPrincipalLegIsOutflow_ShouldThrow()
	{
		AssetMovementLeg[] legs = [ActivityTestFactory.PrincipalLeg(MovementDirection.Outflow)];

		Func<IncomeActivity> act = () => IncomeActivity.Create(
			TenantId.New(),
			PlatformAccountId.New(),
			DateTimeOffset.UtcNow,
			AssetActivityReason.Reward,
			legs,
			ActivityTestFactory.ExternalMetadata(),
			null);

		_ = act.Should().Throw<DomainValidationException>()
			.WithMessage("*only Inflow principal legs*");
	}
}