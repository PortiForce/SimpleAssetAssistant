using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Services.Activity;

public interface IActivityPersistenceService
{
	Task<TypedResult<ActivityId>> PersistNewAsync(
		AssetActivityBase activity,
		string extPrimaryId,
		CancellationToken ct);
}
