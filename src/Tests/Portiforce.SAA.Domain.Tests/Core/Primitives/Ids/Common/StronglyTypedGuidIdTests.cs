namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

/// <summary>
///     Reusable behavioral contract for strongly typed Guid-based IDs.
///     Intended for types like AssetId, AccountId, TenantId, etc.
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
		TId id = this.Empty;

		_ = this.GetValue(id).Should().Be(Guid.Empty);
		_ = this.IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void From_WhenValidGuidProvided_ShouldWrapSameValue()
	{
		Guid value = Guid.NewGuid();

		TId id = this.From(value);

		_ = this.GetValue(id).Should().Be(value);
		_ = this.IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void New_ShouldCreateNonEmptyId()
	{
		TId id = this.New();

		_ = this.GetValue(id).Should().NotBe(Guid.Empty);
		_ = this.IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void Parse_WhenValidGuid_ShouldReturnId()
	{
		Guid value = Guid.NewGuid();

		TId id = this.Parse(value.ToString());

		_ = this.GetValue(id).Should().Be(value);
		_ = this.IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void Parse_WhenEmptyGuid_ShouldThrowFormatException()
	{
		Action act = () => this.Parse(Guid.Empty.ToString());

		_ = act.Should().Throw<FormatException>();
	}

	[Fact]
	public void Parse_WhenInvalidString_ShouldThrowFormatException()
	{
		Action act = () => this.Parse("not-a-guid");

		_ = act.Should().Throw<FormatException>();
	}

	[Fact]
	public void Parse_WhenNullString_ShouldThrow()
	{
		Action act = () => this.Parse(null!);

		_ = act.Should().Throw<Exception>();
	}

	[Fact]
	public void TryParse_WhenNull_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = this.TryParse(null, out TId id);

		_ = ok.Should().BeFalse();
		_ = id.Should().Be(this.Empty);
		_ = this.IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenEmptyString_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = this.TryParse(string.Empty, out TId id);

		_ = ok.Should().BeFalse();
		_ = id.Should().Be(this.Empty);
		_ = this.IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenWhitespace_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = this.TryParse("   ", out TId id);

		_ = ok.Should().BeFalse();
		_ = id.Should().Be(this.Empty);
		_ = this.IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenInvalidString_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = this.TryParse("abc", out TId id);

		_ = ok.Should().BeFalse();
		_ = id.Should().Be(this.Empty);
		_ = this.IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenEmptyGuid_ShouldReturnFalse_AndEmptyId()
	{
		bool ok = this.TryParse(Guid.Empty.ToString(), out TId id);

		_ = ok.Should().BeFalse();
		_ = id.Should().Be(this.Empty);
		_ = this.IsEmpty(id).Should().BeTrue();
	}

	[Fact]
	public void TryParse_WhenValidGuid_ShouldReturnTrue_AndParsedId()
	{
		Guid value = Guid.NewGuid();

		bool ok = this.TryParse(value.ToString(), out TId id);

		_ = ok.Should().BeTrue();
		_ = this.GetValue(id).Should().Be(value);
		_ = this.IsEmpty(id).Should().BeFalse();
	}

	[Fact]
	public void Equality_WhenSameUnderlyingGuid_ShouldBeEqual()
	{
		Guid value = Guid.NewGuid();

		TId left = this.From(value);
		TId right = this.From(value);

		_ = left.Should().Be(right);
		_ = left.GetHashCode().Should().Be(right.GetHashCode());
	}

	[Fact]
	public void Equality_WhenDifferentUnderlyingGuid_ShouldNotBeEqual()
	{
		TId left = this.From(Guid.NewGuid());
		TId right = this.From(Guid.NewGuid());

		_ = left.Should().NotBe(right);
	}

	[Fact]
	public void ToString_WhenValueIsValid_ShouldRoundTripThroughParse()
	{
		TId original = this.New();

		string raw = original.ToString()!;

		_ = raw.Should().NotBeNullOrWhiteSpace();

		TId parsed = this.Parse(raw);

		_ = parsed.Should().Be(original);
	}
}