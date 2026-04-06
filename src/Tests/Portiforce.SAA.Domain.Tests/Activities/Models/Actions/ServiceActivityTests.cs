using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

public sealed class ServiceActivityTests
{
	[Fact]
	public void Create_ShouldSetExpectedProperties()
	{
		TenantId tenantId = TenantId.New();
		PlatformAccountId platformAccountId = PlatformAccountId.New();
		DateTimeOffset occurredAt = DateTimeOffset.UtcNow;
		AssetMovementLeg[] legs = [ActivityTestFactory.PrincipalLeg(MovementDirection.Outflow)];
		ExternalMetadata externalMetadata = ActivityTestFactory.ExternalMetadata();

		ServiceActivity activity = ServiceActivity.Create(
			tenantId,
			platformAccountId,
			occurredAt,
			AssetActivityReason.ServiceFee,
			legs,
			externalMetadata);

		_ = activity.TenantId.Should().Be(tenantId);
		_ = activity.PlatformAccountId.Should().Be(platformAccountId);
		_ = activity.OccurredAt.Should().Be(occurredAt);
		_ = activity.Kind.Should().Be(AssetActivityKind.Service);
		_ = activity.Reason.Should().Be(AssetActivityReason.ServiceFee);
		_ = activity.ServiceType.Should().Be(ServiceType.Custody);
		_ = activity.ExternalMetadata.Should().Be(externalMetadata);
		_ = activity.Legs.Should().Equal(legs);
	}

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

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*only Outflow principal legs*");
	}
}