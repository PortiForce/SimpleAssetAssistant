using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Guards;
using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Flow.Guards;

internal sealed class ActivityIdempotencyGuard(IActivityReadRepository activityReadRepository) : IActivityIdempotencyGuard
{
	public async Task EnsureNotExistsAsync(
		ExternalMetadata metadata,
		AssetActivityKind kind,
		TenantId tenantId,
		PlatformAccountId platformAccountId,
		CancellationToken ct)
	{
		if (metadata == null)
		{
			throw new ArgumentNullException(nameof(metadata));
		}

		string primaryId = metadata.GetPrimaryId();

		// check that record is not added already
		if (metadata.IsExternalIdDriven())
		{
			bool isRecordAlreadyAdded = await activityReadRepository.ExistsByExternalIdAsync(
				metadata.ExternalId,
				kind,
				tenantId,
				platformAccountId,
				ct);

			if (isRecordAlreadyAdded)
			{
				throw new ConflictException($"{kind} activity with externalId '{primaryId}' already exists.");
			}
		}
		else
		{
			bool isRecordAlreadyAdded = await activityReadRepository.ExistsByFingerprintAsync(
				metadata.Fingerprint,
				kind,
				tenantId,
				platformAccountId,
				ct);

			if (isRecordAlreadyAdded)
			{
				throw new ConflictException($"{kind} activity with fingerprint '{primaryId}' already exists.");
			}
		}
	}
}
