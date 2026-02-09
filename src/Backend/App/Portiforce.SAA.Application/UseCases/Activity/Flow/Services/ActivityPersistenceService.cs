using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Application.Interfaces.Services.Activity;
using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Activities;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Flow.Services;

internal sealed class ActivityPersistenceService(
	IActivityWriteRepository activityWriteRepository,
	IUnitOfWork unitOfWork) : IActivityPersistenceService
{
	public async Task<CommandResult<ActivityId>> PersistNewAsync(
		AssetActivityBase activity,
		string extPrimaryId,
		CancellationToken ct)
	{
		if (activity == null)
		{
			throw new ArgumentNullException(nameof(activity));
		}

		AssetActivityKind activityKind = activity.Kind;

		int affectedRows = 0;
		try
		{
			// Persist : do not forget about race conditions here, as activity might be already added
			await activityWriteRepository.AddAsync(activity, ct);
			affectedRows = await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			throw new ConflictException($"{activityKind} activity already exists. Id: {extPrimaryId}");
		}

		if (affectedRows == 0)
		{
			throw new ConflictException($"No changes were persisted (possible concurrency issue). {activityKind} activity Id: {extPrimaryId}");
		}

		return new CommandResult<ActivityId>
		{
			Id = activity.Id,
			Message = $"{activityKind} registered successfully"
		};
	}
}
