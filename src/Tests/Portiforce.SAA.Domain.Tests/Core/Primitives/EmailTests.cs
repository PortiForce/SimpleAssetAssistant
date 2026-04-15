using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives;

public sealed class EmailTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenEmpty_ShouldThrow(string? raw)
	{
		Func<Email> act = () => Email.Create(raw!);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Fact]
	public void Create_WhenLengthExceeds255_ShouldThrow()
	{
		string localPart = new('a', 252);
		string raw = $"{localPart}@x.com";

		Func<Email> act = () => Email.Create(raw);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("plainaddress")]
	[InlineData("user@")]
	[InlineData("@example.com")]
	[InlineData("user@@example.com")]
	[InlineData("user example@example.com")]
	[InlineData("user@example")]
	[InlineData("user@.com")]
	public void Create_WhenInvalidFormat_ShouldThrow(string raw)
	{
		Func<Email> act = () => Email.Create(raw);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("John Doe <user@example.com>")]
	[InlineData("\"John Doe\" <user@example.com>")]
	public void Create_WhenDisplayNameFormatProvided_ShouldThrow(string raw)
	{
		Func<Email> act = () => Email.Create(raw);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("rawData");
	}

	[Theory]
	[InlineData("USER@EXAMPLE.COM", "user@example.com")]
	[InlineData(" User@Example.Com ", "user@example.com")]
	[InlineData("a.b-c_d+1@example.com", "a.b-c_d+1@example.com")]
	public void Create_ShouldTrimAndLowercase(string raw, string expected)
	{
		Email email = Email.Create(raw);

		_ = email.Value.Should().Be(expected);
		_ = email.ToString().Should().Be(expected);
	}

	[Fact]
	public void Create_WhenLengthIs255_ShouldSucceed()
	{
		string localPart = new('a', 243);
		string raw = $"{localPart}@x.com";

		Email email = Email.Create(raw);

		_ = email.Value.Should().Be(raw);
	}

	[Theory]
	[InlineData("user@example.com")]
	[InlineData("first.last@example.com")]
	[InlineData("user+tag@example.com")]
	[InlineData("user_name@example.co.uk")]
	[InlineData("a-b@example.org")]
	public void Create_WhenValidEmail_ShouldSucceed(string raw)
	{
		Email email = Email.Create(raw);

		_ = email.Value.Should().Be(raw.Trim().ToLowerInvariant());
	}

	[Fact]
	public void TryCreate_WhenValid_ShouldReturnTrueAndValue()
	{
		bool result = Email.TryCreate(" User@Example.Com ", out Email email);

		_ = result.Should().BeTrue();
		_ = email.Should().NotBeNull();
		_ = email.Value.Should().Be("user@example.com");
	}

	[Fact]
	public void TryCreate_WhenInvalid_ShouldReturnFalseAndDefault()
	{
		bool result = Email.TryCreate("not-an-email", out Email email);

		_ = result.Should().BeFalse();
		_ = email.Should().BeNull();
	}

	[Fact]
	public void Create_WhenSameNormalizedValue_ShouldBeEqual()
	{
		Email a = Email.Create("USER@EXAMPLE.COM");
		Email b = Email.Create("user@example.com");
		Email c = Email.Create(" user@example.com ");

		_ = a.Should().Be(b);
		_ = b.Should().Be(c);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenDifferentValues_ShouldNotBeEqual()
	{
		Email a = Email.Create("a@example.com");
		Email b = Email.Create("b@example.com");

		_ = a.Should().NotBe(b);
	}
}