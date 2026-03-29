using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Core;

public sealed class EntityEqualityTests
{
	[Fact]
	public void Equals_WhenSameReference_ShouldBeTrue()
	{
		TestEntity entity = new(AssetId.New());

		_ = entity.Equals(entity).Should().BeTrue();
		_ = entity.Equals((object) entity).Should().BeTrue();
		_ = (entity == entity).Should().BeTrue();
		_ = (entity != entity).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenSameTransientReference_ShouldBeTrue()
	{
		TestEntity entity = new(default);

		_ = entity.Equals(entity).Should().BeTrue();
		_ = entity.Equals((object) entity).Should().BeTrue();
		_ = (entity == entity).Should().BeTrue();
		_ = (entity != entity).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenSameTypeAndSameNonEmptyId_ShouldBeTrue()
	{
		AssetId id = AssetId.New();

		TestEntity a = new(id);
		TestEntity b = new(id);

		_ = a.Equals(b).Should().BeTrue();
		_ = b.Equals(a).Should().BeTrue();
		_ = a.Equals((object) b).Should().BeTrue();
		_ = (a == b).Should().BeTrue();
		_ = (a != b).Should().BeFalse();
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenSameTypeButDifferentIds_ShouldBeFalse()
	{
		TestEntity a = new(AssetId.New());
		TestEntity b = new(AssetId.New());

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = a.Equals((object) b).Should().BeFalse();
		_ = (a == b).Should().BeFalse();
		_ = (a != b).Should().BeTrue();
	}

	[Fact]
	public void Equals_WhenDifferentTypesEvenWithSameId_ShouldBeFalse()
	{
		AssetId id = AssetId.New();

		Entity<AssetId> a = new TestEntity(id);
		Entity<AssetId> b = new AnotherEntity(id);

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = a.Equals((object) b).Should().BeFalse();
		_ = (a == b).Should().BeFalse();
		_ = (a != b).Should().BeTrue();
		_ = a.GetHashCode().Should().NotBe(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenDefaultIdAndDifferentInstances_ShouldBeFalse()
	{
		TestEntity a = new(default);
		TestEntity b = new(default);

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = (a == b).Should().BeFalse();
		_ = (a != b).Should().BeTrue();
	}

	[Fact]
	public void Equals_WhenOneIdIsDefaultAndOtherIsNonDefault_ShouldBeFalse()
	{
		TestEntity a = new(default);
		TestEntity b = new(AssetId.New());

		_ = a.Equals(b).Should().BeFalse();
		_ = b.Equals(a).Should().BeFalse();
		_ = (a == b).Should().BeFalse();
		_ = (a != b).Should().BeTrue();
	}

	[Fact]
	public void Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		TestEntity? a = new(AssetId.New());

		_ = a.Equals(null).Should().BeFalse();
		_ = a.Equals((object?) null).Should().BeFalse();
	}

	[Fact]
	public void Operators_WhenComparedWithNull_ShouldBehaveCorrectly()
	{
		TestEntity a = new(AssetId.New());
		TestEntity? nullEntity = null;

		_ = (a == nullEntity).Should().BeFalse();
		_ = (a != nullEntity).Should().BeTrue();
		_ = (nullEntity == a).Should().BeFalse();
		_ = (nullEntity != a).Should().BeTrue();
	}

	[Fact]
	public void Operators_WhenBothNull_ShouldBehaveCorrectly()
	{
		TestEntity? a = null;
		TestEntity? b = null;

		_ = (a == b).Should().BeTrue();
		_ = (a != b).Should().BeFalse();
	}

	[Fact]
	public void Equals_WhenObjectIsDifferentType_ShouldBeFalse()
	{
		TestEntity entity = new(AssetId.New());

		_ = entity.Equals(new object()).Should().BeFalse();
		_ = entity.Equals("test").Should().BeFalse();
	}

	private sealed class TestEntity : Entity<AssetId>
	{
		public TestEntity(AssetId id) : base(id)
		{
		}
	}

	private sealed class AnotherEntity : Entity<AssetId>
	{
		public AnotherEntity(AssetId id) : base(id)
		{
		}
	}
}