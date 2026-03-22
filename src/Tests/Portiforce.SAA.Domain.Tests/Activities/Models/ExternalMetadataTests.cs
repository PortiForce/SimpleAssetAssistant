using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Domain.Tests.Activities.Models;

public sealed class ExternalMetadataTests
{
	[Fact]
	public void Ctor_WhenSourceMissing_ShouldThrow()
	{
		var act = () => new ExternalMetadata(source: "", externalId: "1");
		act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void Ctor_WhenBothExternalIdAndFingerprintMissing_ShouldThrow()
	{
		var act = () => new ExternalMetadata(source: "tests", externalId: null, fingerprint: null);
		act.Should().Throw<DomainValidationException>();
	}

	[Fact]
	public void GetPrimaryId_ShouldPreferExternalId()
	{
		var m = new ExternalMetadata(source: "tests", externalId: "E1", fingerprint: "F1");

		m.IsExternalIdDriven().Should().BeTrue();
		m.GetPrimaryId().Should().Be("E1");
	}

	[Fact]
	public void GetPrimaryId_WhenOnlyFingerprint_ShouldReturnFingerprint()
	{
		var m = new ExternalMetadata(source: "tests", externalId: null, fingerprint: "F1");

		m.IsExternalIdDriven().Should().BeFalse();
		m.GetPrimaryId().Should().Be("F1");
	}
}