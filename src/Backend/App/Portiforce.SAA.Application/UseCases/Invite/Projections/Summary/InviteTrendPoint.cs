namespace Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

public sealed record InviteTrendPoint(
	DateTimeOffset BucketStartUtc,
	int Sent,
	int Accepted,
	int Declined,
	int Failed,
	int Pending,
	int Expired,
	int Revoked);