using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Activities.Rules;

public sealed class ConsistencyRulesTests
{
	[Fact]
	public void EnsureScaleDoesNotExceed_WhenWithinScale_ShouldNotThrow()
	{
		Action act = () => ConsistencyRules.EnsureScaleDoesNotExceed(1.12m, 2, "amount");
		_ = act.Should().NotThrow();
	}

	[Fact]
	public void EnsureScaleDoesNotExceed_WhenExceedsScale_ShouldThrow()
	{
		Action act = () => ConsistencyRules.EnsureScaleDoesNotExceed(1.123m, 2, "amount");
		_ = act.Should().Throw<DomainValidationException>()
			.WithMessage("*max allowed is 2*");
	}
}