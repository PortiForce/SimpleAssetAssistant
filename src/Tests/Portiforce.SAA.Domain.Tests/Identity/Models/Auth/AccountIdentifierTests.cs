using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Domain.Tests.Identity.Models.Auth;

public sealed class AccountIdentifierTests
{
	[Fact]
	public void Create_WhenEmail_ShouldNormalizeValue()
	{
		AccountIdentifier identifier = AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.Email,
			"  USER@Example.COM  ",
			true,
			false);

		_ = identifier.Value.Should().Be("user@example.com");
		_ = identifier.IsVerified.Should().BeTrue();
		_ = identifier.IsPrimary.Should().BeFalse();
	}

	[Fact]
	public void Create_WhenPhone_ShouldNormalizeValue()
	{
		AccountIdentifier identifier = AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.Phone,
			" +38 (050) 123-45-67 ",
			false,
			true);

		_ = identifier.Value.Should().Be("+380501234567");
		_ = identifier.IsVerified.Should().BeFalse();
		_ = identifier.IsPrimary.Should().BeTrue();
	}

	[Fact]
	public void Create_WhenTelegramUserName_ShouldTrimValue()
	{
		AccountIdentifier identifier = AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.TelegramUserName,
			"  @nickname  ",
			false,
			false);

		_ = identifier.Value.Should().Be("@nickname");
	}

	[Fact]
	public void Create_WhenTelegramUserId_ShouldTrimValue()
	{
		AccountIdentifier identifier = AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.TelegramUserId,
			"  123456789  ",
			false,
			false);

		_ = identifier.Value.Should().Be("123456789");
	}

	[Fact]
	public void Create_WhenTenantIdIsEmpty_ShouldThrow()
	{
		Func<AccountIdentifier> act = () => AccountIdentifier.Create(
			TenantId.Empty,
			AccountId.New(),
			AccountIdentifierKind.Email,
			"user@example.com",
			true,
			false);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*TenantId must be defined*");
	}

	[Fact]
	public void Create_WhenAccountIdIsEmpty_ShouldThrow()
	{
		Func<AccountIdentifier> act = () => AccountIdentifier.Create(
			TenantId.New(),
			AccountId.Empty,
			AccountIdentifierKind.Email,
			"user@example.com",
			true,
			false);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*AccountId must be defined*");
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenValueIsEmpty_ShouldThrow(string? value)
	{
		Func<AccountIdentifier> act = () => AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.TelegramUserId,
			value!,
			false,
			false);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*Identifier value is required*");
	}

	[Fact]
	public void Create_WhenEmailIsInvalid_ShouldThrow()
	{
		Func<AccountIdentifier> act = () => AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.Email,
			"not-an-email",
			false,
			false);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Create_WhenPhoneIsInvalid_ShouldThrow()
	{
		Func<AccountIdentifier> act = () => AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.Phone,
			"0501234567",
			false,
			false);

		_ = act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Create_WhenCustomIdProvided_ShouldUseIt()
	{
		AccountIdentifierId id = AccountIdentifierId.New();

		AccountIdentifier identifier = AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.Email,
			"user@example.com",
			true,
			true,
			id);

		_ = identifier.Id.Should().Be(id);
	}

	[Fact]
	public void Create_WhenNoIdProvided_ShouldGenerateNonEmptyId()
	{
		AccountIdentifier identifier = AccountIdentifier.Create(
			TenantId.New(),
			AccountId.New(),
			AccountIdentifierKind.Email,
			"user@example.com",
			true,
			true);

		_ = identifier.Id.IsEmpty.Should().BeFalse();
	}
}