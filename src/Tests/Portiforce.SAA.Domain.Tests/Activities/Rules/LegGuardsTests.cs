using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Activities.Rules;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Activities.Models.Actions;

namespace Portiforce.SAA.Domain.Tests.Activities.Rules;

public sealed class LegGuardsTests
{
	[Fact]
	public void EnsureTradeOrExchangeShape_WhenNoInflowPrincipal_ShouldThrow()
	{
		AssetMovementLeg[] legs = new[]
		{
			ActivityTestFactory.PrincipalLeg(MovementDirection.Outflow),
			ActivityTestFactory.PrincipalLeg(MovementDirection.Outflow), ActivityTestFactory.FeeLegOutflow()
		};

		Action act = () => LegGuards.EnsureTradeOrExchangeShape(legs);
		_ = act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void EnsureTradeOrExchangeShape_WhenSameAssetInAndOut_ShouldThrow()
	{
		AssetId asset = AssetId.New();

		AssetMovementLeg[] legs = new[]
		{
			ActivityTestFactory.PrincipalLeg(MovementDirection.Outflow, asset),
			ActivityTestFactory.PrincipalLeg(MovementDirection.Inflow, asset)
		};

		Action act = () => LegGuards.EnsureTradeOrExchangeShape(legs);
		_ = act.Should().Throw<DomainValidationException>()
			.WithMessage("*assets must differ*");
	}
}