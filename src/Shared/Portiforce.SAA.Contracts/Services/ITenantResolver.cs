using Portiforce.SAA.Contracts.Models.Client;

namespace Portiforce.SAA.Contracts.Services;

public interface ITenantResolver
{
	Task<TenantResolution?> ResolveByPrefixAsync(string prefix, CancellationToken ct);
}
