namespace Portiforce.SAA.Contracts.Contexts;

public interface ITenantContext
{
	bool IsLanding { get; set; }

	string? Prefix { get; set; }

	Guid? TenantId { get; set; }

	string? PublicName { get; set; }
}