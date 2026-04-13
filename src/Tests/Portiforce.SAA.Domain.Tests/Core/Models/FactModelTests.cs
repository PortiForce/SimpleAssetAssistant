using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Core.Models;

public sealed class FactModelTests
{
	[Fact]
	public void Constructor_WhenIdProvided_ShouldSetId()
	{
		AssetId id = AssetId.New();

		TestFact fact = new(id);

		_ = fact.Id.Should().Be(id);
	}

	[Fact]
	public void Equals_WhenSameReference_ShouldBeTrue()
	{
		TestFact fact = new(AssetId.New());

		_ = fact.Equals(fact).Should().BeTrue();
		_ = fact.Should().Be(fact);
	}

	[Fact]
	public void Equals_WhenSameTransientReference_ShouldBeTrue()
	{
		TestFact fact = new(default);

		_ = fact.Equals(fact).Should().BeTrue();
		_ = fact.Should().Be(fact);
	}

	[Fact]
	public void Equals_WhenSameTypeAndSameNonDefaultId_ShouldBeTrue()
	{
		AssetId id = AssetId.New();

		TestFact a = new(id);
		TestFact b = new(id);

		_ = a.Equals(b).Should().BeTrue();
		_ = b.Equals(a).Should().BeTrue();
		_ = a.Should().Be(b);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenSameTypeButDifferentIds_ShouldBeFalse()
	{
		TestFact a = new(AssetId.New());
		TestFact b = new(AssetId.New());

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = a.Should().NotBe(b);
	}

	[Fact]
	public void Equals_WhenDifferentTypesEvenWithSameId_ShouldBeFalse()
	{
		AssetId id = AssetId.New();

		Fact<AssetId> a = new TestFact(id);
		Fact<AssetId> b = new AnotherTestFact(id);

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = a.Should().NotBe(b);
	}

	[Fact]
	public void Equals_WhenDefaultIdAndDifferentInstances_ShouldBeFalse()
	{
		TestFact a = new(default);
		TestFact b = new(default);

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = a.Should().NotBe(b);
	}

	[Fact]
	public void Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		TestFact fact = new(AssetId.New());

		_ = fact.Equals(null).Should().BeFalse();
	}

	[Fact]
	public void GetHashCode_WhenIdIsDefault_AndInstancesAreDifferent_ShouldBeDifferent()
	{
		TestFact a = new(default);
		TestFact b = new(default);

		_ = a.GetHashCode().Should().NotBe(b.GetHashCode());
	}

	[Fact]
	public void GetHashCode_WhenIdIsDefault_ShouldBeStableForSameInstance()
	{
		TestFact fact = new(default);

		int first = fact.GetHashCode();
		int second = fact.GetHashCode();

		_ = second.Should().Be(first);
	}

	[Fact]
	public void GetHashCode_WhenIdIsNonDefault_ShouldBeStableAndIdBased()
	{
		AssetId id = AssetId.New();

		TestFact a = new(id);
		TestFact b = new(id);

		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	private sealed record TestFact : Fact<AssetId>
	{
		public TestFact(AssetId id)
			: base(id)
		{
		}
	}

	private sealed record TestFactWithParameterlessConstructor : Fact<AssetId>
	{
	}

	private sealed record AnotherTestFact : Fact<AssetId>
	{
		public AnotherTestFact(AssetId id)
			: base(id)
		{
		}
	}
}