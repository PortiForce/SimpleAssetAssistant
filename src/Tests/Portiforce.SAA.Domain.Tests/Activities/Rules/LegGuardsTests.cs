using Portiforce.SAA.Core.Activities.Enums;
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
		var legs = new[]
		{
			ActivityTestFactory.PrincipalLeg(direction: MovementDirection.Outflow),
			ActivityTestFactory.PrincipalLeg(direction: MovementDirection.Outflow),
			ActivityTestFactory.FeeLegOutflow()
		};

		var act = () => LegGuards.EnsureTradeOrExchangeShape(legs);
		act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void EnsureTradeOrExchangeShape_WhenSameAssetInAndOut_ShouldThrow()
	{
		var asset = AssetId.New();

		var legs = new[]
		{
			ActivityTestFactory.PrincipalLeg(direction: MovementDirection.Outflow, assetId: asset),
			ActivityTestFactory.PrincipalLeg(direction: MovementDirection.Inflow, assetId: asset)
		};

		var act = () => LegGuards.EnsureTradeOrExchangeShape(legs);
		act.Should().Throw<DomainValidationException>()
			.WithMessage("*assets must differ*");
	}
}