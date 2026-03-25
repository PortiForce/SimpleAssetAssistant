namespace Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

public sealed record InviteOutcomeBreakdown(
	int Accepted,
	int Declined,
	int Failed,
	int Pending,
	int Expired,
	int Revoked);