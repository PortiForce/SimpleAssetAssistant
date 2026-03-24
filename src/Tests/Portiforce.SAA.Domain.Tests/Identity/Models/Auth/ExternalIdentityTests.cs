using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Domain.Tests.Identity.Models.Auth;

public sealed class ExternalIdentityTests
{
	[Fact]
	public void Create_ShouldTrimProviderSubject()
	{
		ExternalIdentity externalIdentity = ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			"  sub-123  ");

		_ = externalIdentity.ProviderSubject.Should().Be("sub-123");
		_ = externalIdentity.IsPrimary.Should().BeFalse();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WhenProviderSubjectIsEmpty_ShouldThrow(string? providerSubject)
	{
		Func<ExternalIdentity> act = () => ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			providerSubject!);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*ProviderSubject is required*");
	}

	[Fact]
	public void Create_WhenProviderSubjectTooLong_ShouldThrow()
	{
		string longSub = new('a', EntityConstraints.CommonSettings.ProviderSubjectMaxLength + 1);

		Func<ExternalIdentity> act = () => ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			longSub);

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*ProviderSubject is too long*");
	}

	[Fact]
	public void Create_WhenAccountIdIsEmpty_ShouldThrow()
	{
		Func<ExternalIdentity> act = () => ExternalIdentity.Create(
			AccountId.Empty,
			TenantId.New(),
			AuthProvider.Google,
			"sub-123");

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*AccountId must be defined*");
	}

	[Fact]
	public void Create_WhenTenantIdIsEmpty_ShouldThrow()
	{
		Func<ExternalIdentity> act = () => ExternalIdentity.Create(
			AccountId.New(),
			TenantId.Empty,
			AuthProvider.Google,
			"sub-123");

		_ = act.Should()
			.Throw<DomainValidationException>()
			.WithMessage("*TenantId must be defined*");
	}

	[Fact]
	public void Create_WhenIsPrimaryIsTrue_ShouldSetFlag()
	{
		ExternalIdentity externalIdentity = ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			"sub-123",
			true);

		_ = externalIdentity.IsPrimary.Should().BeTrue();
	}

	[Fact]
	public void MarkPrimary_And_UnmarkPrimary_ShouldToggle()
	{
		ExternalIdentity externalIdentity = ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			"sub");

		externalIdentity.MarkPrimary();
		_ = externalIdentity.IsPrimary.Should().BeTrue();

		externalIdentity.UnmarkPrimary();
		_ = externalIdentity.IsPrimary.Should().BeFalse();
	}
}