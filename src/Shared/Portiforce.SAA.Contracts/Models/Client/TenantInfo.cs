namespace Portiforce.SAA.Contracts.Models.Client;

public sealed class TenantInfo
{
	public string? Prefix { get; set; }

	public bool IsLanding { get; set; }

	public string BaseDomain { get; set; } = string.Empty;
}
