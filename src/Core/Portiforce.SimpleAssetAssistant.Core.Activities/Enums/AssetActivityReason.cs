namespace Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

public enum AssetActivityReason : byte
{
	Buy = 1,
	Sell = 2,

	Reward = 20,
	Staking = 21,
	Referral = 22,
	Mining = 23,

	Conversion = 30,

	ServiceFee = 40,

	Burn = 50,

	UserCorrectionWrongData = 60
}
