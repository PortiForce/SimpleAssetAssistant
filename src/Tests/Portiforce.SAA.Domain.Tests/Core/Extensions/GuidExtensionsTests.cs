using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Domain.Tests.Core.Extensions;

public sealed class GuidExtensionsTests
{
	[Fact]
	public void New_ShouldReturnNonEmptyGuid()
	{
		Guid result = GuidExtensions.New();

		_ = result.Should().NotBe(Guid.Empty);
	}

	[Fact]
	public void New_ShouldReturnVersion7Guid()
	{
		Guid result = GuidExtensions.New();

		string text = result.ToString("D");

		_ = text[14].Should().Be('7');
	}

	[Fact]
	public void New_CalledTwice_ShouldReturnDifferentGuids()
	{
		Guid first = GuidExtensions.New();
		Guid second = GuidExtensions.New();

		_ = first.Should().NotBe(second);
	}

	[Fact]
	public void TryParse_WhenValidDFormat_ShouldReturnTrueAndParsedGuid()
	{
		Guid expected = Guid.NewGuid();
		string raw = expected.ToString("D");

		bool result = GuidExtensions.TryParse(raw, out Guid value);

		_ = result.Should().BeTrue();
		_ = value.Should().Be(expected);
	}

	[Fact]
	public void TryParse_WhenValidNFormat_ShouldReturnTrueAndParsedGuid()
	{
		Guid expected = Guid.NewGuid();
		string raw = expected.ToString("N");

		bool result = GuidExtensions.TryParse(raw, out Guid value);

		_ = result.Should().BeTrue();
		_ = value.Should().Be(expected);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("not-a-guid")]
	[InlineData("12345678-1234-1234-1234-12345678901")]
	[InlineData("12345678-1234-1234-1234-1234567890123")]
	public void TryParse_WhenInvalid_ShouldReturnFalseAndEmptyGuid(string? raw)
	{
		bool result = GuidExtensions.TryParse(raw, out Guid value);

		_ = result.Should().BeFalse();
		_ = value.Should().Be(Guid.Empty);
	}

	[Fact]
	public void ToString_ShouldReturnDFormat()
	{
		Guid guid = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");

		string result = GuidExtensions.ToString(guid);

		_ = result.Should().Be("00112233-4455-6677-8899-aabbccddeeff");
	}

	[Fact]
	public void ToString_ResultShouldRoundTripThroughTryParse()
	{
		Guid original = Guid.NewGuid();
		string text = GuidExtensions.ToString(original);

		bool parsed = GuidExtensions.TryParse(text, out Guid result);

		_ = parsed.Should().BeTrue();
		_ = result.Should().Be(original);
	}

	[Fact]
	public void ToString_ShouldReturnLowercaseHyphenatedString()
	{
		Guid guid = Guid.Parse("AABBCCDD-EEFF-0011-2233-445566778899");

		string result = GuidExtensions.ToString(guid);

		_ = result.Should().Be("aabbccdd-eeff-0011-2233-445566778899");
	}
}