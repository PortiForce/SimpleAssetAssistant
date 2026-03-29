using System.Globalization;

using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Domain.Tests.Core.Extensions;

public sealed class DateExtensionsTests
{
	[Fact]
	public void FromUnixSeconds_WhenZero_ShouldReturnUnixEpoch()
	{
		DateTimeOffset result = DateExtensions.FromUnixSeconds(0);

		_ = result.Should().Be(DateTimeOffset.UnixEpoch);
	}

	[Fact]
	public void FromUnixSeconds_WhenPositiveValue_ShouldReturnExpectedDate()
	{
		DateTimeOffset result = DateExtensions.FromUnixSeconds(1);

		_ = result.Should().Be(DateTimeOffset.UnixEpoch.AddSeconds(1));
	}

	[Fact]
	public void FromUnixSeconds_WhenNegativeValue_ShouldReturnExpectedDate()
	{
		DateTimeOffset result = DateExtensions.FromUnixSeconds(-1);

		_ = result.Should().Be(DateTimeOffset.UnixEpoch.AddSeconds(-1));
	}

	[Theory]
	[InlineData("0", 0)]
	[InlineData("1", 1)]
	[InlineData("-1", -1)]
	[InlineData("1710000000", 1710000000)]
	public void TryFromUnixSeconds_WhenValidNumericString_ShouldReturnTrue(string raw, long expectedSeconds)
	{
		bool result = DateExtensions.TryFromUnixSeconds(raw, out DateTimeOffset value);

		_ = result.Should().BeTrue();
		_ = value.Should().Be(DateTimeOffset.FromUnixTimeSeconds(expectedSeconds));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("abc")]
	[InlineData("12.5")]
	[InlineData("1,000")]
	[InlineData("2024-01-01")]
	public void TryFromUnixSeconds_WhenInvalidString_ShouldReturnFalse(string? raw)
	{
		bool result = DateExtensions.TryFromUnixSeconds(raw!, out DateTimeOffset value);

		_ = result.Should().BeFalse();
		_ = value.Should().Be(default);
	}

	[Fact]
	public void TryFromUnixSeconds_WhenValueHasWhitespace_ShouldReturnTrue()
	{
		bool result = DateExtensions.TryFromUnixSeconds("  123  ", out DateTimeOffset value);

		_ = result.Should().BeTrue();
		_ = value.Should().Be(DateTimeOffset.FromUnixTimeSeconds(123));
	}

	[Fact]
	public void TryFromUnixSeconds_WhenNumericValueIsOutOfRange_ShouldReturnFalse()
	{
		bool result = DateExtensions.TryFromUnixSeconds(long.MaxValue.ToString(CultureInfo.InvariantCulture), out _);

		_ = result.Should().BeFalse();
	}

	[Fact]
	public void FromUnixSeconds_WhenValueIsTooLarge_ShouldThrow()
	{
		Func<DateTimeOffset> act = () => DateExtensions.FromUnixSeconds(long.MaxValue);

		_ = act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void FromUnixSeconds_WhenValueIsTooSmall_ShouldThrow()
	{
		Func<DateTimeOffset> act = () => DateExtensions.FromUnixSeconds(long.MinValue);

		_ = act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(-1)]
	[InlineData(1700000000)]
	public void TryFromUnixSeconds_WhenSuccessful_ShouldMatchFromUnixSeconds(long seconds)
	{
		bool result = DateExtensions.TryFromUnixSeconds(
			seconds.ToString(CultureInfo.InvariantCulture),
			out DateTimeOffset value);

		_ = result.Should().BeTrue();
		_ = value.Should().Be(DateExtensions.FromUnixSeconds(seconds));
	}
}