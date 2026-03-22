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
	public void Equals_WhenSameReference_ShouldBeTrue()
	{
		var entity = new TestEntity(AssetId.New());

		entity.Equals(entity).Should().BeTrue();
		entity.Equals((object)entity).Should().BeTrue();
		(entity == entity).Should().BeTrue();
		(entity != entity).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenSameTransientReference_ShouldBeTrue()
	{
		var entity = new TestEntity(default);

		entity.Equals(entity).Should().BeTrue();
		entity.Equals((object)entity).Should().BeTrue();
		(entity == entity).Should().BeTrue();
		(entity != entity).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenSameTypeAndSameNonEmptyId_ShouldBeTrue()
	{
		var id = AssetId.New();

		var a = new TestEntity(id);
		var b = new TestEntity(id);

		a.Equals(b).Should().BeTrue();
		b.Equals(a).Should().BeTrue();
		a.Equals((object)b).Should().BeTrue();
		(a == b).Should().BeTrue();
		(a != b).Should().BeFalse();
		a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenSameTypeButDifferentIds_ShouldBeFalse()
	{
		var a = new TestEntity(AssetId.New());
		var b = new TestEntity(AssetId.New());

		a.Equals(b).Should().BeFalse();
		b.Equals(a).Should().BeFalse();
		a.Equals((object)b).Should().BeFalse();
		(a == b).Should().BeFalse();
		(a != b).Should().BeTrue();
	}

	[Fact]
	public void Equals_WhenDifferentTypesEvenWithSameId_ShouldBeFalse()
	{
		var id = AssetId.New();

		Entity<AssetId> a = new TestEntity(id);
		Entity<AssetId> b = new AnotherEntity(id);

		a.Equals(b).Should().BeFalse();
		b.Equals(a).Should().BeFalse();
		a.Equals((object)b).Should().BeFalse();
		(a == b).Should().BeFalse();
		(a != b).Should().BeTrue();
		a.GetHashCode().Should().NotBe(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenDefaultIdAndDifferentInstances_ShouldBeFalse()
	{
		var a = new TestEntity(default);
		var b = new TestEntity(default);

		a.Equals(b).Should().BeFalse();
		b.Equals(a).Should().BeFalse();
		(a == b).Should().BeFalse();
		(a != b).Should().BeTrue();
	}

	[Fact]
	public void Equals_WhenOneIdIsDefaultAndOtherIsNonDefault_ShouldBeFalse()
	{
		var a = new TestEntity(default);
		var b = new TestEntity(AssetId.New());

		a.Equals(b).Should().BeFalse();
		b.Equals(a).Should().BeFalse();
		(a == b).Should().BeFalse();
		(a != b).Should().BeTrue();
	}

	[Fact]
	public void Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		var a = new TestEntity(AssetId.New());

		a.Equals((Entity<AssetId>?)null).Should().BeFalse();
		a.Equals((object?)null).Should().BeFalse();
	}

	[Fact]
	public void Operators_WhenComparedWithNull_ShouldBehaveCorrectly()
	{
		TestEntity a = new(AssetId.New());
		TestEntity? nullEntity = null;

		(a == nullEntity).Should().BeFalse();
		(a != nullEntity).Should().BeTrue();
		(nullEntity == a).Should().BeFalse();
		(nullEntity != a).Should().BeTrue();
	}

	[Fact]
	public void Operators_WhenBothNull_ShouldBehaveCorrectly()
	{
		TestEntity? a = null;
		TestEntity? b = null;

		(a == b).Should().BeTrue();
		(a != b).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenObjectIsDifferentType_ShouldBeFalse()
	{
		var entity = new TestEntity(AssetId.New());

		entity.Equals(new object()).Should().BeFalse();
		entity.Equals("test").Should().BeFalse();
	}
}