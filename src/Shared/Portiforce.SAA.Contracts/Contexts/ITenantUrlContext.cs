namespace Portiforce.SAA.Contracts.Contexts;

public interface ITenantUrlContext
{
	bool IsTenantResolved();

	string[] GetDomainParts();
}