using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed class TenantRestrictedPlatform
{
	public TenantRestrictedPlatform(TenantId tenantId, PlatformId platformId)
	{
		if (tenantId.IsEmpty)
		{
			throw new DomainValidationException(nameof(tenantId));
		}

		if (platformId.IsEmpty)
		{
			throw new DomainValidationException(nameof(platformId));
		}

		TenantId = tenantId;
		PlatformId = platformId;
	}


	// Private Empty Constructor for EF Core
	private TenantRestrictedPlatform()
	{

	}

	public TenantId TenantId { get; init; }
	public PlatformId PlatformId { get; init; }
}
