namespace Portiforce.SAA.Contracts.Models.Client.Invite.Summary;

public sealed record InviteSummaryCardsResponse(
	int TotalSent,
	int Accepted,
	int Declined,
	int Failed,
	int Pending,
	int Expired,
	int Revoked,
	decimal AcceptanceRatePercent);