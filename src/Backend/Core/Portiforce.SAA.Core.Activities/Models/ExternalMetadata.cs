using Portiforce.SAA.Core.Exceptions;

namespace Portiforce.SAA.Core.Activities.Models;

public sealed record ExternalMetadata 
{
	public ExternalMetadata(
		string source,
		string? externalId = null,
		string? fingerprint = null,
		string? notes = null)
	{
		if (string.IsNullOrWhiteSpace(source))
		{
			throw new DomainValidationException("Source is required.");
		}

		if (string.IsNullOrWhiteSpace(externalId) && string.IsNullOrWhiteSpace(fingerprint))
		{
			throw new DomainValidationException("Either externalId or fingerprint are required.");
		}

		Source = source;
		ExternalId = externalId;
		Fingerprint = fingerprint;
		Notes = notes;
	}

	// Private Empty Constructor for EF Core
	private ExternalMetadata() { }

	public string Source { get; init; } = null!;
	public string? ExternalId { get; init; }
	public string? Fingerprint { get; init; }
	public string? Notes { get; init; }

	public bool IsExternalIdDriven()
	{
		return !string.IsNullOrWhiteSpace(ExternalId);
	}

	public string GetPrimaryId()
	{
		return IsExternalIdDriven() ? ExternalId : Fingerprint;
	}
}
