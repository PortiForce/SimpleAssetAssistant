using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Guards;

public interface IActivityIdempotencyGuard
{
	Task EnsureNotExistsAsync(
		ExternalMetadata metadata,
		AssetActivityKind kind,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		CancellationToken ct);
}
