namespace Portiforce.SAA.Contracts.Models.Client.Invite.Summary;

public sealed record InviteTierUsageResponse(
	string TierCode,
	int Used,
	int Left,
	int TotalLimit,
	decimal UsedPercent,
	decimal LeftPercent);