using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Rules;

public static class ActivityGuards
{
	/// <summary>
	/// Validates that a given Reason is compatible with a given ActivityKind.
	/// IMPORTANT: Transfer is not "reasoned" in this model; do not call this for Transfer.
	/// </summary>
	public static bool IsAllowed(AssetActivityKind kind, AssetActivityReason reason) =>
		kind switch
		{
			AssetActivityKind.Trade => reason is AssetActivityReason.Buy or AssetActivityReason.Sell,

			AssetActivityKind.Exchange => reason is AssetActivityReason.Conversion,

			AssetActivityKind.Income => reason is AssetActivityReason.Reward
				or AssetActivityReason.Staking
				or AssetActivityReason.Mining
				or AssetActivityReason.Referral,

			AssetActivityKind.Service => reason is AssetActivityReason.ServiceFee,

			AssetActivityKind.Burn => reason is AssetActivityReason.Burn,

			// Transfer is not a ReasonedActivity in your current model.
			AssetActivityKind.Transfer => false,

			_ => false
		};

	public static void EnsureReasonKindPairAllowed(AssetActivityKind kind, AssetActivityReason reason)
	{
		if (kind == AssetActivityKind.Transfer)
		{
			throw new ArgumentException(
				$"'{nameof(AssetActivityKind.Transfer)}' is not a reasoned activity. " +
				$"Do not validate it via {nameof(IsAllowed)}; validate transfer via Direction/LegGuards instead.");
		}

		if (!IsAllowed(kind, reason))
		{
			throw new ArgumentException($"Reason '{reason}' is not allowed for '{kind}' activity.");
		}
	}
}
