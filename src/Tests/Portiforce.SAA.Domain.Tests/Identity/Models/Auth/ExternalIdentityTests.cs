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
		var ei = ExternalIdentity.Create(
			accountId: AccountId.New(),
			tenantId: TenantId.New(),
			provider: AuthProvider.Google,
			providerSubject: "  sub-123  ");

		ei.ProviderSubject.Should().Be("sub-123");
		ei.IsPrimary.Should().BeFalse();
	}

	[Fact]
	public void Create_WhenProviderSubjectTooLong_ShouldThrow()
	{
		var longSub = new string('a', EntityConstraints.CommonSettings.ProviderSubjectMaxLength + 1);

		var act = () => ExternalIdentity.Create(
			accountId: AccountId.New(),
			tenantId: TenantId.New(),
			provider: AuthProvider.Google,
			providerSubject: longSub);

		act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void MarkPrimary_And_UnmarkPrimary_ShouldToggle()
	{
		var ei = ExternalIdentity.Create(
			accountId: AccountId.New(),
			tenantId: TenantId.New(),
			provider: AuthProvider.Google,
			providerSubject: "sub");

		ei.MarkPrimary();
		ei.IsPrimary.Should().BeTrue();

		ei.UnmarkPrimary();
		ei.IsPrimary.Should().BeFalse();
	}
}