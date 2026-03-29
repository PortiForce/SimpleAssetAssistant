namespace Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

public sealed record InviteSummary(
	InviteSummaryRange Range,
	InviteTrendBucket TrendBucket,
	DateTimeOffset FromUtc,
	DateTimeOffset ToUtc,
	InviteSummaryCards Cards,
	InviteOutcomeBreakdown OutcomeBreakdown,
	IReadOnlyList<InviteTierUsage> TierUsage,
	InviteTrend Trend);