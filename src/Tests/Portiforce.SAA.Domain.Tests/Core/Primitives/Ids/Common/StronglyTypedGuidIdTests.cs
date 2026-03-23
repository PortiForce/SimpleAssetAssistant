namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

/// <summary>
/// Reusable behavioral contract for strongly typed Guid-based IDs.
/// Intended for types like AssetId, AccountId, TenantId, etc.
/// </summary>
/// <typeparam name="TId">Strongly typed identifier type.</typeparam>
public abstract class StronglyTypedGuidIdTests<TId>
	where TId : struct
{
	protected abstract TId Empty { get; }
	protected abstract TId New();
	protected abstract TId From(Guid value);
	protected abstract TId Parse(string raw);
	protected abstract bool TryParse(string? raw, out TId id);
	protected abstract Guid GetValue(TId id);
	protected abstract bool IsEmpty(TId id);

	[Fact]
	public void Empty_ShouldRepresentEmptyGuid()
	{
		TId id = Empty;

		GetValue(id).Should().Be(Guid.Empty);
		IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void From_WhenValidGuidProvided_ShouldWrapSameValue()
	{
		Guid value = Guid.NewGuid();

		TId id = From(value);

		GetValue(id).Should().Be(value);
		IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void New_ShouldCreateNonEmptyId()
	{
		TId id = New();

		GetValue(id).Should().NotBe(Guid.Empty);
		IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void Parse_WhenValidGuid_ShouldReturnId()
	{
		Guid value = Guid.NewGuid();

		TId id = Parse(value.ToString());

		GetValue(id).Should().Be(value);
		IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void Parse_WhenEmptyGuid_ShouldThrowFormatException()
	{
		Action act = () => Parse(Guid.Empty.ToString());

		act.Should().Throw<FormatException>();
	}

	[Fact]
	public void Parse_WhenInvalidString_ShouldThrowFormatException()
	{
		Action act = () => Parse("not-a-guid");

		act.Should().Throw<FormatException>();
	}

	[Fact]
	public void Parse_WhenNullString_ShouldThrow()
	{
		Action act = () => Parse(null!);

		act.Should().Throw<Exception>();
	}

	[Fact]
	public void TryParse_WhenNull_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = TryParse(null, out TId id);

		ok.Should().BeFalse();
		id.Should().Be(Empty);
		IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenEmptyString_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = TryParse(string.Empty, out TId id);

		ok.Should().BeFalse();
		id.Should().Be(Empty);
		IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenWhitespace_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = TryParse("   ", out TId id);

		ok.Should().BeFalse();
		id.Should().Be(Empty);
		IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenInvalidString_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = TryParse("abc", out TId id);

		ok.Should().BeFalse();
		id.Should().Be(Empty);
		IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenEmptyGuid_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = TryParse(Guid.Empty.ToString(), out TId id);

		ok.Should().BeFalse();
		id.Should().Be(Empty);
		IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenValidGuid_ShouldReturnTrue_AndParsedId()
	{
		Guid value = Guid.NewGuid();

		bool ok = TryParse(value.ToString(), out TId id);

		ok.Should().BeTrue();
		GetValue(id).Should().Be(value);
		IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void Equality_WhenSameUnderlyingGuid_ShouldBeEqual()
	{
		Guid value = Guid.NewGuid();

		TId left = From(value);
		TId right = From(value);

		left.Should().Be(right);
		left.GetHashCode().Should().Be(right.GetHashCode());
	}

	[Fact]
	public void Equality_WhenDifferentUnderlyingGuid_ShouldNotBeEqual()
	{
		TId left = From(Guid.NewGuid());
		TId right = From(Guid.NewGuid());

		left.Should().NotBe(right);
	}

	[Fact]
	public void ToString_WhenValueIsValid_ShouldRoundTripThroughParse()
	{
		TId original = New();

		string raw = original.ToString()!;

		raw.Should().NotBeNullOrWhiteSpace();

		TId parsed = Parse(raw);

		parsed.Should().Be(original);
	}
}