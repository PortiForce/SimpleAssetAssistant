namespace Portiforce.SAA.Core.Identity.Models.Client;

public sealed record TenantSecuritySettings
{
	public bool EnforceTwoFactor { get; init; }
}
