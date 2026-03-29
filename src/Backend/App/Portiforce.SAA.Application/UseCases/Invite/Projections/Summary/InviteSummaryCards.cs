namespace Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

public sealed record InviteSummaryCards(
	int TotalSent,
	int Accepted,
	int Declined,
	int Failed,
	int Pending,
	int Expired,
	int Revoked,
	decimal AcceptanceRatePercent);