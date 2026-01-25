using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed class TenantRestrictedAsset
{
	public TenantRestrictedAsset(TenantId tenantId, AssetId assetId)
	{
		if (tenantId.IsEmpty)
		{
			throw new DomainValidationException(nameof(tenantId));
		}

		if (assetId.IsEmpty)
		{
			throw new DomainValidationException(nameof(assetId));
		}

		TenantId = tenantId;
		AssetId = assetId;
	}

	// Private Empty Constructor for EF Core
	private TenantRestrictedAsset()
	{

	}

	public TenantId TenantId { get; init; }
	public AssetId AssetId { get; init; }
}
