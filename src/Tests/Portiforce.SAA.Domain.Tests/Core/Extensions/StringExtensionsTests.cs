using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Domain.Tests.Core.Extensions;

public sealed class StringExtensionsTests
{
	[Fact]
	public void Truncate_WhenNull_ShouldReturnPlaceholder()
	{
		string? value = null;

		string result = value.Truncate();

		_ = result.Should().Be("<null-or-empty>");
	}

	[Fact]
	public void Truncate_WhenEmpty_ShouldReturnPlaceholder()
	{
		string value = string.Empty;

		string result = value.Truncate();

		_ = result.Should().Be("<null-or-empty>");
	}

	[Fact]
	public void Truncate_WhenShorterThanMaxLength_ShouldReturnOriginal()
	{
		string value = "Hello";

		string result = value.Truncate(10);

		_ = result.Should().Be("Hello");
	}

	[Fact]
	public void Truncate_WhenExactlyMaxLength_ShouldReturnOriginal()
	{
		string value = "Hello";

		string result = value.Truncate(5);

		_ = result.Should().Be("Hello");
	}

	[Fact]
	public void Truncate_WhenLongerThanMaxLength_ShouldReturnTruncatedWithEllipsisWithinLimit()
	{
		string value = "HelloWorld";

		string result = value.Truncate(8);

		_ = result.Should().Be("Hello...");
		_ = result.Length.Should().Be(8);
	}

	[Fact]
	public void Truncate_WhenUsingDefaultMaxLength_ShouldUse64AsFinalLength()
	{
		string value = new('A', 65);

		string result = value.Truncate();

		_ = result.Length.Should().Be(64);
		_ = result.Should().Be(new string('A', 61) + "...");
	}

	[Fact]
	public void Truncate_WhenMaxLengthIsThree_ShouldReturnOnlyEllipsis()
	{
		string value = "HelloWorld";

		string result = value.Truncate(3);

		_ = result.Should().Be("...");
		_ = result.Length.Should().Be(3);
	}

	[Fact]
	public void Truncate_WhenMaxLengthIsTwo_ShouldReturnTwoDots()
	{
		string value = "HelloWorld";

		string result = value.Truncate(2);

		_ = result.Should().Be("..");
		_ = result.Length.Should().Be(2);
	}

	[Fact]
	public void Truncate_WhenMaxLengthIsOne_ShouldReturnOneDot()
	{
		string value = "HelloWorld";

		string result = value.Truncate(1);

		_ = result.Should().Be(".");
		_ = result.Length.Should().Be(1);
	}

	[Fact]
	public void Truncate_WhenMaxLengthIsZero_ShouldReturnEmptyString()
	{
		string value = "HelloWorld";

		string result = value.Truncate(0);

		_ = result.Should().BeEmpty();
	}

	[Fact]
	public void Truncate_WhenMaxLengthIsNegative_ShouldThrow()
	{
		string value = "Hello";

		Func<string> act = () => value.Truncate(-1);

		_ = act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void Truncate_WhenWhitespaceOnly_ShouldReturnWhitespace()
	{
		string value = "   ";

		string result = value.Truncate();

		_ = result.Should().Be("   ");
	}
}