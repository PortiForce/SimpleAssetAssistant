using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record ExecutableActivity(ActivityId Id) : ReasonedActivity(Id)
{
	public CompletionType CompletionType { get; init; } = CompletionType.FullyCompleted;
}
