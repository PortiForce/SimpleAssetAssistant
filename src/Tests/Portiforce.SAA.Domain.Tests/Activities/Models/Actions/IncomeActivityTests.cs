using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

public sealed class IncomeActivityTests
{
	[Fact]
	public void Create_ShouldSetExpectedProperties()
	{
		TenantId tenantId = TenantId.New();
		PlatformAccountId platformAccountId = PlatformAccountId.New();
		DateTimeOffset occurredAt = DateTimeOffset.UtcNow;
		AssetMovementLeg[] legs = [ActivityTestFactory.PrincipalLeg(MovementDirection.Inflow)];
		ExternalMetadata externalMetadata = ActivityTestFactory.ExternalMetadata();

		IncomeActivity activity = IncomeActivity.Create(
			tenantId,
			platformAccountId,
			occurredAt,
			AssetActivityReason.Reward,
			legs,
			externalMetadata,
			null);

		_ = activity.TenantId.Should().Be(tenantId);
		_ = activity.PlatformAccountId.Should().Be(platformAccountId);
		_ = activity.OccurredAt.Should().Be(occurredAt);
		_ = activity.Kind.Should().Be(AssetActivityKind.Income);
		_ = activity.Reason.Should().Be(AssetActivityReason.Reward);
		_ = activity.ExternalMetadata.Should().Be(externalMetadata);
		_ = activity.Legs.Should().Equal(legs);
	}

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

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*only Inflow principal legs*");
	}
}