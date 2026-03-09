using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Core;

public sealed class EntityEqualityTests
{
	private sealed class TestEntity : Entity<AssetId>
	{
		public TestEntity(AssetId id) : base(id) { }
	}

	private sealed class AnotherEntity : Entity<AssetId>
	{
		public AnotherEntity(AssetId id) : base(id) { }
	}

	[Fact]
	public void Equals_WhenSameTypeAndSameNonEmptyId_ShouldBeTrue()
	{
		var id = AssetId.New();

		var a = new TestEntity(id);
		var b = new TestEntity(id);

		(a == b).Should().BeTrue();
		a.Equals(b).Should().BeTrue();
		a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenDifferentTypesEvenWithSameId_ShouldBeFalse()
	{
		var id = AssetId.New();

		var a = new TestEntity(id);
		var b = new AnotherEntity(id);

		a.Equals(b).Should().BeFalse();
		(a == (Entity<AssetId>)b).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenDefaultIdAndDifferentInstances_ShouldBeFalse()
	{
		var a = new TestEntity(default);
		var b = new TestEntity(default);

		a.Equals(b).Should().BeFalse();
		(a == b).Should().BeFalse();
	}
}