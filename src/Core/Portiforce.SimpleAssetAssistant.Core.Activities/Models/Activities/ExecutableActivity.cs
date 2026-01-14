using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record ExecutableActivity : ReasonedActivity
{
	public CompletionType CompletionType { get; init; } = CompletionType.FullyCompleted;
}
