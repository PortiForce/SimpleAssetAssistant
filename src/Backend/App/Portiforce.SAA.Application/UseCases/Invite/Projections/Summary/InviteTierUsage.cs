namespace Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

public sealed record InviteTierUsage(
	string TierCode,
	int Used,
	int Left,
	int TotalLimit,
	decimal UsedPercent,
	decimal LeftPercent);