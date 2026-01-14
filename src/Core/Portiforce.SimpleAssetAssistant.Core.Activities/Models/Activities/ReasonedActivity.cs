using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record ReasonedActivity : AssetActivityBase
{
	public required AssetActivityReason Reason { get; init; }
}
