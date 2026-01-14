namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models;

public sealed record ExternalMetadata 
{
	public ExternalMetadata(
		string source,
		string? externalId = null,
		string? fingerprint = null,
		string? notes = null)
	{
		Source = source;
		ExternalId = externalId;
		Fingerprint = fingerprint;
		Notes = notes;
	}

	public string Source { get; }
	public string? ExternalId { get; }
	public string? Fingerprint { get; }
	public string? Notes { get; }
}
