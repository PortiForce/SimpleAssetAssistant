using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Domain.Tests.Primitives;

public sealed class InviteTargetTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Restore_WhenValueIsEmpty_ShouldThrow(string? raw)
	{
		Func<InviteTarget> act = () => InviteTarget.Restore(
			raw!,
			InviteChannel.Email,
			InviteTargetKind.Email);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.Be("value");
	}

	[Theory]
	[InlineData(InviteChannel.Email, InviteTargetKind.None)]
	[InlineData(InviteChannel.Email, InviteTargetKind.Phone)]
	[InlineData(InviteChannel.Email, InviteTargetKind.TelegramUserName)]
	[InlineData(InviteChannel.Email, InviteTargetKind.TelegramUserId)]
	[InlineData(InviteChannel.Telegram, InviteTargetKind.Email)]
	[InlineData(InviteChannel.Telegram, InviteTargetKind.Phone)]
	[InlineData(InviteChannel.AppleAccount, InviteTargetKind.TelegramUserName)]
	[InlineData(InviteChannel.AppleAccount, InviteTargetKind.TelegramUserId)]
	public void Restore_WhenChannelAndKindCombinationIsInvalid_ShouldThrow(
		InviteChannel channel,
		InviteTargetKind kind)
	{
		Func<InviteTarget> act = () => InviteTarget.Restore("test-value", channel, kind);

		_ = act.Should()
			.Throw<ArgumentException>()
			.Which.ParamName.Should()
			.BeNull();
	}

	[Theory]
	[InlineData("plainaddress")]
	[InlineData("user@")]
	[InlineData("@example.com")]
	[InlineData("user@@example.com")]
	[InlineData("user example@example.com")]
	[InlineData("user@example")]
	[InlineData("user@.com")]
	public void Email_WhenInvalidFormat_ShouldThrow(string raw)
	{
		Func<InviteTarget> act = () => InviteTarget.Email(raw);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Theory]
	[InlineData("plainaddress")]
	[InlineData("user@")]
	[InlineData("@example.com")]
	[InlineData("user@@example.com")]
	public void AppleEmail_WhenInvalidFormat_ShouldThrow(string raw)
	{
		Func<InviteTarget> act = () => InviteTarget.AppleEmail(raw);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Theory]
	[InlineData("not-a-phone")]
	[InlineData("abc")]
	[InlineData("123")]
	public void ApplePhone_WhenInvalidFormat_ShouldThrow(string raw)
	{
		Func<InviteTarget> act = () => InviteTarget.ApplePhone(raw);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Theory]
	[InlineData("USER@EXAMPLE.COM", "user@example.com")]
	[InlineData(" User@Example.Com ", "user@example.com")]
	[InlineData("a.b-c_d+1@example.com", "a.b-c_d+1@example.com")]
	public void Email_ShouldNormalizeValue(string raw, string expected)
	{
		InviteTarget target = InviteTarget.Email(raw);

		_ = target.Channel.Should().Be(InviteChannel.Email);
		_ = target.Kind.Should().Be(InviteTargetKind.Email);
		_ = target.Value.Should().Be(expected);
		_ = target.IsEmpty.Should().BeFalse();
	}

	[Theory]
	[InlineData("USER@EXAMPLE.COM", "user@example.com")]
	[InlineData(" User@Example.Com ", "user@example.com")]
	public void AppleEmail_ShouldNormalizeValue(string raw, string expected)
	{
		InviteTarget target = InviteTarget.AppleEmail(raw);

		_ = target.Channel.Should().Be(InviteChannel.AppleAccount);
		_ = target.Kind.Should().Be(InviteTargetKind.Email);
		_ = target.Value.Should().Be(expected);
		_ = target.IsEmpty.Should().BeFalse();
	}

	[Theory]
	[InlineData("  @john_doe  ", "john_doe")]
	[InlineData("@@john", "john")]
	[InlineData("john", "john")]
	[InlineData(" john ", "john")]
	public void TelegramUserName_ShouldTrimAndRemoveLeadingAt(string raw, string expected)
	{
		InviteTarget target = InviteTarget.TelegramUserName(raw);

		_ = target.Channel.Should().Be(InviteChannel.Telegram);
		_ = target.Kind.Should().Be(InviteTargetKind.TelegramUserName);
		_ = target.Value.Should().Be(expected);
		_ = target.IsEmpty.Should().BeFalse();
	}

	[Theory]
	[InlineData("123456789", "123456789")]
	[InlineData(" 123456789 ", "123456789")]
	[InlineData("  000123  ", "000123")]
	public void TelegramUserId_ShouldTrimValue(string raw, string expected)
	{
		InviteTarget target = InviteTarget.TelegramUserId(raw);

		_ = target.Channel.Should().Be(InviteChannel.Telegram);
		_ = target.Kind.Should().Be(InviteTargetKind.TelegramUserId);
		_ = target.Value.Should().Be(expected);
		_ = target.IsEmpty.Should().BeFalse();
	}

	[Fact]
	public void ApplePhone_WhenValid_ShouldSucceed()
	{
		InviteTarget target = InviteTarget.ApplePhone(" +44 7700 900123 ");

		_ = target.Channel.Should().Be(InviteChannel.AppleAccount);
		_ = target.Kind.Should().Be(InviteTargetKind.Phone);
		_ = target.Value.Should().Be(PhoneNumber.Create(" +44 7700 900123 ").Value);
		_ = target.IsEmpty.Should().BeFalse();
	}

	[Theory]
	[InlineData("user@example.com")]
	[InlineData("first.last@example.com")]
	[InlineData("user+tag@example.com")]
	public void Email_WhenValid_ShouldSucceed(string raw)
	{
		InviteTarget target = InviteTarget.Email(raw);

		_ = target.Channel.Should().Be(InviteChannel.Email);
		_ = target.Kind.Should().Be(InviteTargetKind.Email);
		_ = target.Value.Should().Be(raw.Trim().ToLowerInvariant());
	}

	[Theory]
	[InlineData("user@example.com")]
	[InlineData("first.last@example.com")]
	public void AppleEmail_WhenValid_ShouldSucceed(string raw)
	{
		InviteTarget target = InviteTarget.AppleEmail(raw);

		_ = target.Channel.Should().Be(InviteChannel.AppleAccount);
		_ = target.Kind.Should().Be(InviteTargetKind.Email);
		_ = target.Value.Should().Be(raw.Trim().ToLowerInvariant());
	}

	[Theory]
	[InlineData("@john")]
	[InlineData("john")]
	[InlineData(" john_doe ")]
	public void TelegramUserName_WhenValid_ShouldSucceed(string raw)
	{
		InviteTarget target = InviteTarget.TelegramUserName(raw);

		_ = target.Channel.Should().Be(InviteChannel.Telegram);
		_ = target.Kind.Should().Be(InviteTargetKind.TelegramUserName);
		_ = target.Value.Should().NotBeNullOrWhiteSpace();
	}

	[Theory]
	[InlineData("12345")]
	[InlineData(" 12345 ")]
	[InlineData("000123")]
	public void TelegramUserId_WhenValid_ShouldSucceed(string raw)
	{
		InviteTarget target = InviteTarget.TelegramUserId(raw);

		_ = target.Channel.Should().Be(InviteChannel.Telegram);
		_ = target.Kind.Should().Be(InviteTargetKind.TelegramUserId);
		_ = target.Value.Should().NotBeNullOrWhiteSpace();
	}

	[Fact]
	public void Restore_WhenEmailDataProvided_ShouldMatchEmailFactory()
	{
		InviteTarget restored = InviteTarget.Restore(
			" User@Example.Com ",
			InviteChannel.Email,
			InviteTargetKind.Email);

		InviteTarget created = InviteTarget.Email(" User@Example.Com ");

		_ = restored.Should().Be(created);
	}

	[Fact]
	public void Restore_WhenAppleEmailDataProvided_ShouldMatchAppleEmailFactory()
	{
		InviteTarget restored = InviteTarget.Restore(
			" User@Example.Com ",
			InviteChannel.AppleAccount,
			InviteTargetKind.Email);

		InviteTarget created = InviteTarget.AppleEmail(" User@Example.Com ");

		_ = restored.Should().Be(created);
	}

	[Fact]
	public void Restore_WhenApplePhoneDataProvided_ShouldMatchApplePhoneFactory()
	{
		InviteTarget restored = InviteTarget.Restore(
			" +44 7700 900123 ",
			InviteChannel.AppleAccount,
			InviteTargetKind.Phone);

		InviteTarget created = InviteTarget.ApplePhone(" +44 7700 900123 ");

		_ = restored.Should().Be(created);
	}

	[Fact]
	public void Restore_WhenTelegramUserNameDataProvided_ShouldMatchTelegramUserNameFactory()
	{
		InviteTarget restored = InviteTarget.Restore(
			"  @john  ",
			InviteChannel.Telegram,
			InviteTargetKind.TelegramUserName);

		InviteTarget created = InviteTarget.TelegramUserName("  @john  ");

		_ = restored.Should().Be(created);
	}

	[Fact]
	public void Restore_WhenTelegramUserIdDataProvided_ShouldMatchTelegramUserIdFactory()
	{
		InviteTarget restored = InviteTarget.Restore(
			"  12345  ",
			InviteChannel.Telegram,
			InviteTargetKind.TelegramUserId);

		InviteTarget created = InviteTarget.TelegramUserId("  12345  ");

		_ = restored.Should().Be(created);
	}

	[Fact]
	public void Create_WhenSameNormalizedEmailValue_ShouldBeEqual()
	{
		InviteTarget a = InviteTarget.Email("USER@EXAMPLE.COM");
		InviteTarget b = InviteTarget.Email("user@example.com");
		InviteTarget c = InviteTarget.Email(" user@example.com ");

		_ = a.Should().Be(b);
		_ = b.Should().Be(c);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenSameNormalizedTelegramUserName_ShouldBeEqual()
	{
		InviteTarget a = InviteTarget.TelegramUserName("@john");
		InviteTarget b = InviteTarget.TelegramUserName("john");
		InviteTarget c = InviteTarget.TelegramUserName("  @@john  ");

		_ = a.Should().Be(b);
		_ = b.Should().Be(c);
		_ = a.GetHashCode().Should().Be(b.GetHashCode());
	}

	[Fact]
	public void Create_WhenDifferentChannels_ShouldNotBeEqual()
	{
		InviteTarget a = InviteTarget.Email("user@example.com");
		InviteTarget b = InviteTarget.AppleEmail("user@example.com");

		_ = a.Should().NotBe(b);
	}

	[Fact]
	public void Create_WhenDifferentKinds_ShouldNotBeEqual()
	{
		InviteTarget a = InviteTarget.TelegramUserName("john");
		InviteTarget b = InviteTarget.TelegramUserId("john");

		_ = a.Should().NotBe(b);
	}

	[Fact]
	public void Create_WhenDifferentValues_ShouldNotBeEqual()
	{
		InviteTarget a = InviteTarget.Email("a@example.com");
		InviteTarget b = InviteTarget.Email("b@example.com");

		_ = a.Should().NotBe(b);
	}
}