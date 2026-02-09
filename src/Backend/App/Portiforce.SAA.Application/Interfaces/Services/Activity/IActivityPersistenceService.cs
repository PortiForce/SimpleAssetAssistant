using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Core.Activities.Models.Activities;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Services.Activity;

public interface IActivityPersistenceService
{
	Task<CommandResult<ActivityId>> PersistNewAsync(
		AssetActivityBase activity,
		string extPrimaryId,
		CancellationToken ct);
}
