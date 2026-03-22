using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Application.Interfaces.Services.Activity;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Flow.Services;

internal sealed class ActivityPersistenceService(
	IActivityWriteRepository activityWriteRepository,
	IUnitOfWork unitOfWork) : IActivityPersistenceService
{
	public async Task<TypedResult<ActivityId>> PersistNewAsync(
		AssetActivityBase activity,
		string extPrimaryId,
		CancellationToken ct)
	{
		if (activity.TenantId == TenantId.Empty)
		{
			return TypedResult<ActivityId>.Fail(ResultError.Validation("TenantId is required."));
		}

		if (activity.PlatformAccountId == PlatformAccountId.Empty)
		{
			return TypedResult<ActivityId>.Fail(ResultError.Validation("Platform Account is required."));
		}

		if (string.IsNullOrWhiteSpace(extPrimaryId))
		{
			return TypedResult<ActivityId>.Fail(ResultError.Validation("extPrimaryId is required."));
		}

		var activityKind = activity.Kind;

		try
		{
			await activityWriteRepository.AddAsync(activity, ct);
			var affectedRows = await unitOfWork.SaveChangesAsync(ct);

			if (affectedRows <= 0)
			{
				// Treat as persistence failure; conflict is arguable, but keep it simple.
				return TypedResult<ActivityId>.Fail(ResultError.Conflict(
					$"No changes were persisted for {activityKind} (possible concurrency issue). Id: {extPrimaryId}",
					details: new Dictionary<string, object?>
					{
						["kind"] = activityKind.ToString(),
						["extPrimaryId"] = extPrimaryId
					}));
			}

			return TypedResult<ActivityId>.Ok(activity.Id);
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<ActivityId>.Fail(ResultError.Conflict(
				$"{activityKind} activity already exists. Id: {extPrimaryId}",
				details: new Dictionary<string, object?>
				{
					["kind"] = activityKind.ToString(),
					["extPrimaryId"] = extPrimaryId
				}));
		}
	}
}