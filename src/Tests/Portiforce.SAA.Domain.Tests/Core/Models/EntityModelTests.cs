using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Core.Models;

public sealed class EntityModelTests
{
	[Fact]
	public void Constructor_WhenIdProvided_ShouldSetId()
	{
		AssetId id = AssetId.New();

		TestEntity entity = new(id);

		_ = entity.Id.Should().Be(id);
	}

	[Fact]
	public void GetHashCode_WhenIdIsDefault_AndInstancesAreDifferent_ShouldBeDifferent()
	{
		TestEntity a = new(default);
		TestEntity b = new(default);

		_ = a.GetHashCode().Should().NotBe(b.GetHashCode());
	}

	[Fact]
	public void GetHashCode_WhenIdIsDefault_ShouldBeStableForSameInstance()
	{
		TestEntity entity = new(default);

		int first = entity.GetHashCode();
		int second = entity.GetHashCode();

		_ = second.Should().Be(first);
	}

	[Fact]
	public void GetHashCode_WhenIdIsNonDefault_ShouldBeStableForSameInstance()
	{
		TestEntity entity = new(AssetId.New());

		int first = entity.GetHashCode();
		int second = entity.GetHashCode();

		_ = second.Should().Be(first);
	}

	[Fact]
	public void Equals_WhenSameTypeAndSameNonDefaultId_ShouldBeTrue()
	{
		AssetId id = AssetId.New();

		TestEntity a = new(id);
		TestEntity b = new(id);

		_ = a.Should().Be(b);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Equals_WhenDifferentDerivedTypesAndSameId_ShouldBeFalse()
	{
		AssetId id = AssetId.New();

		Entity<AssetId> a = new TestEntity(id);
		Entity<AssetId> b = new AnotherTestEntity(id);

		_ = a.Equals(b).Should().BeFalse();
		_ = a.Should().NotBe(b);
	}

	private sealed class TestEntity : Entity<AssetId>
	{
		public TestEntity(AssetId id)
			: base(id)
		{
		}
	}

	private sealed class TestEntityWithParameterlessConstructor : Entity<AssetId>
	{
	}

	private sealed class AnotherTestEntity : Entity<AssetId>
	{
		public AnotherTestEntity(AssetId id)
			: base(id)
		{
		}
	}
}