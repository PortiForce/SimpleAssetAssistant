using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Activities.Models;

public sealed class ExternalMetadataTests
{
	[Fact]
	public void Ctor_WhenSourceMissing_ShouldThrow()
	{
		Func<ExternalMetadata> act = () => new ExternalMetadata("", "1");
		_ = act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void Ctor_WhenBothExternalIdAndFingerprintMissing_ShouldThrow()
	{
		Func<ExternalMetadata> act = () => new ExternalMetadata("tests");
		_ = act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void GetPrimaryId_ShouldPreferExternalId()
	{
		ExternalMetadata m = new("tests", "E1", "F1");

		_ = m.IsExternalIdDriven().Should().BeTrue();
		_ = m.GetPrimaryId().Should().Be("E1");
	}

	[Fact]
	public void GetPrimaryId_WhenOnlyFingerprint_ShouldReturnFingerprint()
	{
		ExternalMetadata m = new("tests", null, "F1");

		_ = m.IsExternalIdDriven().Should().BeFalse();
		_ = m.GetPrimaryId().Should().Be("F1");
	}
}