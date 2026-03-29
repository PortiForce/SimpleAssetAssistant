namespace Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

public sealed record InviteTrend(
	InviteTrendBucket Bucket,
	IReadOnlyList<InviteTrendPoint> Points);