using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

public abstract record ReasonedActivity(ActivityId Id) : AssetActivityBase(Id)
{
	public required AssetActivityReason Reason { get; init; }
}
