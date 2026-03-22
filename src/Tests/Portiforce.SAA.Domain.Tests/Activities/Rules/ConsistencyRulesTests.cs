using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Activities.Rules;

public sealed class ConsistencyRulesTests
{
	[Fact]
	public void EnsureScaleDoesNotExceed_WhenWithinScale_ShouldNotThrow()
	{
		var act = () => ConsistencyRules.EnsureScaleDoesNotExceed(1.12m, maxDecimals: 2, paramName: "amount");
		act.Should().NotThrow();
	}

	[Fact]
	public void EnsureScaleDoesNotExceed_WhenExceedsScale_ShouldThrow()
	{
		var act = () => ConsistencyRules.EnsureScaleDoesNotExceed(1.123m, maxDecimals: 2, paramName: "amount");
		act.Should().Throw<DomainValidationException>()
			.WithMessage("*max allowed is 2*");
	}
}