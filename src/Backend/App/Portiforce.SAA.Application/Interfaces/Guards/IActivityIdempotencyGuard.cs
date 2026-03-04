using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Guards;

public interface IActivityIdempotencyGuard
{
	Task EnsureNotExistsAsync(
		ExternalMetadata metadata,
		AssetActivityKind kind,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		CancellationToken ct);
}
