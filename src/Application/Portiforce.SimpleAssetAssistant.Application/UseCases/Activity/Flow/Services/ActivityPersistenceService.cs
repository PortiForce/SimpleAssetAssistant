using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Activity;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Services;

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
