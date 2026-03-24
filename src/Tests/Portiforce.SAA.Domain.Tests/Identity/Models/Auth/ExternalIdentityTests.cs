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
		ExternalIdentity ei = ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			"  sub-123  ");

		_ = ei.ProviderSubject.Should().Be("sub-123");
		_ = ei.IsPrimary.Should().BeFalse();
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

		_ = act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void MarkPrimary_And_UnmarkPrimary_ShouldToggle()
	{
		ExternalIdentity ei = ExternalIdentity.Create(
			AccountId.New(),
			TenantId.New(),
			AuthProvider.Google,
			"sub");

		ei.MarkPrimary();
		_ = ei.IsPrimary.Should().BeTrue();

		ei.UnmarkPrimary();
		_ = ei.IsPrimary.Should().BeFalse();
	}
}