using Portiforce.SAA.Core.Activities.Enums;

namespace Portiforce.SAA.Core.Activities.Rules;

public static class ActivityGuards
{
	/// <summary>
	/// Validates that a given Reason is compatible with a given ActivityKind.
	/// IMPORTANT: Transfer is not "reasoned" in this model; do not call this for Transfer.
	/// </summary>
	public static bool IsReasonActivityPairAllowed(AssetActivityKind activityKind, AssetActivityReason reason) =>
		activityKind switch
		{
			// todo feature: trade and exchange entities are quire tricky, maybe they should share same Buyy/Sell/Conversion flows
			AssetActivityKind.Trade => reason is AssetActivityReason.Buy or AssetActivityReason.Sell,
			AssetActivityKind.Exchange => reason is AssetActivityReason.Conversion,

			AssetActivityKind.Income => reason is AssetActivityReason.Reward
				or AssetActivityReason.Staking
				or AssetActivityReason.Mining
				or AssetActivityReason.Referral,

			AssetActivityKind.Service => reason is AssetActivityReason.ServiceFee,

			AssetActivityKind.Burn => reason is AssetActivityReason.Burn,

			AssetActivityKind.UserCorrection => reason is AssetActivityReason.UserCorrectionWrongData,

			// Transfer is not a ReasonedActivity in your current model.
			AssetActivityKind.Transfer => false,

			_ => false
		};

	public static void EnsureReasonKindPairAllowed(AssetActivityKind activityKind, AssetActivityReason reason)
	{
		if (activityKind == AssetActivityKind.Transfer)
		{
			throw new ArgumentException(
				$"'{nameof(AssetActivityKind.Transfer)}' is not a reasoned activity. " +
				$"Do not validate it via {nameof(IsReasonActivityPairAllowed)}; validate transfer via Direction/LegGuards instead.");
		}

		if (!IsReasonActivityPairAllowed(activityKind, reason))
		{
			throw new ArgumentException($"Reason '{reason}' is not allowed for '{activityKind}' activity.");
		}
	}
}
