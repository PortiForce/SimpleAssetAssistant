using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Activity;

public interface IActivityPersistenceService
{
	Task<BaseCreateCommandResult<ActivityId>> PersistNewAsync(
		AssetActivityBase activity,
		string extPrimaryId,
		CancellationToken ct);
}
