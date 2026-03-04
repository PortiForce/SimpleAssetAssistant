using Portiforce.SAA.Contracts.Contexts;

namespace Portiforce.SAA.Web.Services;

public sealed class TenantContext : ITenantContext
{
	public bool IsLanding { get; set; }
	public string? Prefix { get; set; }
	public Guid? TenantId { get; set; }
	public string? PublicName { get; set; }
}
