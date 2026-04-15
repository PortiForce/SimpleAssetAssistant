using Portiforce.SAA.Core.Activities.Enums;

namespace Portiforce.SAA.Core.Activities.Rules;

public static class ActivityGuards
{
	/// <summary>
	///     Determines whether the specified combination of activity kind and reason is allowed according to predefined
	///     business rules.
	/// </summary>
	/// <remarks>
	///     The allowed combinations are determined by business logic and may change as the model evolves. Not
	///     all activity kinds support all reasons; for example, transfer activities are not considered reasoned activities in
	///     the current model.
	/// </remarks>
	/// <param name="activityKind">
	///     The type of asset activity to evaluate. Specifies the general category of the activity, such as trade, exchange,
	///     income, or service.
	/// </param>
	/// <param name="reason">
	///     The reason associated with the asset activity. Indicates the specific cause or context for the activity, such as
	///     buy, sell, conversion, or reward.
	/// </param>
	/// <returns>true if the specified activity kind and reason pair is permitted; otherwise, false.</returns>
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