using Portiforce.SimpleAssetAssistant.Core.Exceptions;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models;

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

	public string Source { get; }
	public string? ExternalId { get; }
	public string? Fingerprint { get; }
	public string? Notes { get; }

	public bool IsExternalIdDriven()
	{
		return !string.IsNullOrWhiteSpace(ExternalId);
	}

	public string GetPrimaryId()
	{
		return IsExternalIdDriven() ? ExternalId : Fingerprint;
	}
}
