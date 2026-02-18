namespace Portiforce.SAA.Contracts.Contexts;

public interface ITenantContext
{
	public bool IsLanding { get; set; }
	public string? Prefix { get; set; }
	public Guid? TenantId { get; set; }
	public string? PublicName { get; set; }
}
