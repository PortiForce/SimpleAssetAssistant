namespace Portiforce.SAA.Contracts.Models.Client.Invite.Summary;

public sealed record InviteTrendPointResponse(
	DateTimeOffset BucketStartUtc,
	int Sent,
	int Accepted,
	int Declined,
	int Failed,
	int Pending,
	int Expired,
	int Revoked);