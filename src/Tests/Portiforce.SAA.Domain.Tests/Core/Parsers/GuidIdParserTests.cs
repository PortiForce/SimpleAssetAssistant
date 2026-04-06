using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Parsers;

namespace Portiforce.SAA.Domain.Tests.Core.Parsers;

public sealed class GuidIdParserTests
{
	[Fact]
	public void Parse_WhenGuidIsValidAndNonEmpty_ShouldReturnCreatedId()
	{
		Guid guid = Guid.NewGuid();
		string raw = guid.ToString("D");

		TestId result = GuidIdParser.Parse(
			raw,
			static g => new TestId(g),
			"TestId");

		_ = result.Value.Should().Be(guid);
	}

	[Fact]
	public void Parse_WhenGuidIsInvalid_ShouldThrowFormatException()
	{
		Func<TestId> act = () => GuidIdParser.Parse(
			"not-a-guid",
			static g => new TestId(g),
			"TestId");

		_ = act.Should()
			.Throw<FormatException>()
			.WithMessage("*valid non-empty TestId GUID*");
	}

	[Fact]
	public void Parse_WhenGuidIsEmpty_ShouldThrowFormatException()
	{
		string raw = Guid.Empty.ToString("D");

		Func<TestId> act = () => GuidIdParser.Parse(
			raw,
			static g => new TestId(g),
			"TestId");

		_ = act.Should()
			.Throw<FormatException>()
			.WithMessage("*valid non-empty TestId GUID*");
	}

	[Fact]
	public void Parse_WhenParsingFails_ShouldNotInvokeFactory()
	{
		bool factoryCalled = false;

		Func<TestId> act = () => GuidIdParser.Parse(
			"invalid",
			g =>
			{
				factoryCalled = true;
				return new TestId(g);
			},
			"TestId");

		_ = act.Should().Throw<FormatException>();
		_ = factoryCalled.Should().BeFalse();
	}

	[Fact]
	public void Parse_WhenParsingFails_ShouldIncludeTruncatedInputInMessage()
	{
		string raw = new('A', 100);

		Func<TestId> act = () => GuidIdParser.Parse(
			raw,
			static g => new TestId(g),
			"TestId");

		_ = act.Should()
			.Throw<FormatException>()
			.Which.Message.Should()
			.Contain("Input (truncated):")
			.And.Contain(raw.Truncate());
	}

	[Fact]
	public void TryParse_WhenGuidIsValidAndNonEmpty_ShouldReturnTrueAndCreatedId()
	{
		Guid guid = Guid.NewGuid();
		string raw = guid.ToString("D");

		bool result = GuidIdParser.TryParse(
			raw,
			static g => new TestId(g),
			new TestId(Guid.Empty),
			out TestId id);

		_ = result.Should().BeTrue();
		_ = id.Value.Should().Be(guid);
	}

	[Fact]
	public void TryParse_WhenGuidIsInvalid_ShouldReturnFalseAndEmptyValue()
	{
		TestId empty = new(Guid.Empty);

		bool result = GuidIdParser.TryParse(
			"invalid",
			static g => new TestId(g),
			empty,
			out TestId id);

		_ = result.Should().BeFalse();
		_ = id.Should().Be(empty);
	}

	[Fact]
	public void TryParse_WhenGuidIsEmpty_ShouldReturnFalseAndEmptyValue()
	{
		TestId empty = new(Guid.Empty);

		bool result = GuidIdParser.TryParse(
			Guid.Empty.ToString("D"),
			static g => new TestId(g),
			empty,
			out TestId id);

		_ = result.Should().BeFalse();
		_ = id.Should().Be(empty);
	}

	[Fact]
	public void TryParse_WhenParsingFails_ShouldNotInvokeFactory()
	{
		bool factoryCalled = false;
		TestId empty = new(Guid.Empty);

		bool result = GuidIdParser.TryParse(
			"invalid",
			g =>
			{
				factoryCalled = true;
				return new TestId(g);
			},
			empty,
			out TestId id);

		_ = result.Should().BeFalse();
		_ = id.Should().Be(empty);
		_ = factoryCalled.Should().BeFalse();
	}

	[Fact]
	public void TryParse_WhenGuidIsEmpty_ShouldNotInvokeFactory()
	{
		bool factoryCalled = false;
		TestId empty = new(Guid.Empty);

		bool result = GuidIdParser.TryParse(
			Guid.Empty.ToString("D"),
			g =>
			{
				factoryCalled = true;
				return new TestId(g);
			},
			empty,
			out TestId id);

		_ = result.Should().BeFalse();
		_ = id.Should().Be(empty);
		_ = factoryCalled.Should().BeFalse();
	}

	[Fact]
	public void TryParse_WhenRawIsNull_ShouldReturnFalseAndEmptyValue()
	{
		TestId empty = new(Guid.Empty);

		bool result = GuidIdParser.TryParse<TestId>(
			null,
			static g => new TestId(g),
			empty,
			out TestId id);

		_ = result.Should().BeFalse();
		_ = id.Should().Be(empty);
	}

	private sealed record TestId(Guid Value);
}